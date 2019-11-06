using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Api.Extensions;
using WeStop.Api.Infra.Hubs;
using WeStop.Core.Services;

namespace WeStop.Api.Infra.Timers
{
    public sealed class GameTimer
    {
        private static readonly IDictionary<Guid, int> _gamesRoundsTimes = new Dictionary<Guid, int>();
        private static readonly ConcurrentDictionary<Guid, Timer> _timers = new ConcurrentDictionary<Guid, Timer>();
        private readonly IHubContext<GameHub> _gameHub;
        private readonly GameManager _gameManager;

        public GameTimer(IHubContext<GameHub> gameHub, GameManager gameManager)
        {
            _gameHub = gameHub;
            _gameManager = gameManager;

            OnRoundTimeElapsed += async (gameId, roundNumber, currentTime, hub) =>
            {
                await hub.Clients.GameRoundGroup(gameId, roundNumber).SendAsync("round_time_elapsed", currentTime);
            };

            OnRoundTimeStoped += (gameId, roundNumber, hub) =>
            {
                StartSendAnswersTimer(gameId, roundNumber);
            };

            OnRoundTimeOver += async (gameId, roundNumber, hub) =>
            {
                await hub.Clients.GameRoundGroup(gameId, roundNumber).SendAsync("round_stoped", new
                {
                    reason = "time_over"
                });

                StartSendAnswersTimer(gameId, roundNumber);
            };

            OnSendAnswersTimeOver += async (gameId, roundNumber, hub) =>
            {
                await StartValidationForNextThemeAsync(gameId, roundNumber);
            };

            OnValidationTimeElapsed += async (gameId, roundNumber, currentTime, theme, hub) =>
            {
                await hub.Clients.GameRoundGroup(gameId, roundNumber).SendAsync("validation_time_elapsed", new
                {
                    currentTime
                });
            };

            OnValidationTimeOver += async (gameId, roundNumber, theme, hub) =>
            {
                await hub.Clients.GameRoundGroup(gameId, roundNumber).SendAsync("validation_time_over");
                StartSendValidationsTimer(gameId, roundNumber, theme);
            };

            OnSendValidationTimeOver += async (gameId, roundNumber, theme) =>
            {
                await _gameManager.FinishValidationsForThemeAsync(gameId, theme);
                await StartValidationForNextThemeAsync(gameId, roundNumber);
            };
        }

        public async Task StartValidationForNextThemeAsync(Guid gameId, int roundNumber)
        {
            RemoveGameTimer(gameId);
            var themeToValidate = await _gameManager.StartValidationForNextThemeAsync(gameId);

            if (!string.IsNullOrEmpty(themeToValidate))
            {
                var playersValidations = await _gameManager.GetPlayersDefaultValidationsAsync(gameId, themeToValidate);

                foreach (var (playerId, validations, totalValidations, validationsNumber) in playersValidations)
                {
                    string connectionId = ConnectionBinding.GetPlayerConnectionId(gameId, playerId);

                    await _gameHub.Clients.Client(connectionId).SendAsync("validation_started", new
                    {
                        theme = themeToValidate,
                        validations,
                        totalValidations,
                        validationsNumber
                    });
                }

                StartValidationTimer(gameId, roundNumber, themeToValidate);
            }
            else
            {
                await _gameManager.FinishCurrentRoundAsync(gameId, async (game) =>
                {
                    var roundScoreboard = game.GetScoreboard(game.CurrentRoundNumber);
                    if (game.IsFinalRound())
                    {
                        var winners = game.GetWinners();
                        await _gameHub.Clients.GameRoundGroup(gameId, roundNumber).SendAsync("game_finished", new
                        {
                            lastRoundScoreboard = roundScoreboard,
                            winners
                        });
                    }
                    else
                    {
                        await _gameHub.Clients.GameRoundGroup(gameId, roundNumber).SendAsync("round_finished", new
                        {
                            scoreboard = roundScoreboard
                        });
                    }

                    foreach (var player in game.Players)
                    {
                        var playerPontuationInRound = player.GetPontuationsInRound(game.CurrentRoundNumber);
                        var playerConnectionId = ConnectionBinding.GetPlayerConnectionId(game.Id, player.Id);
                        await _gameHub.Clients.Client(playerConnectionId).SendAsync("receive_my_pontuations_in_round", new
                        {
                            roundNumber = game.CurrentRoundNumber,
                            pontuations = playerPontuationInRound
                        });
                    }
                });
            }
        }

        public Action<Guid, int, int, IHubContext<GameHub>> OnRoundTimeElapsed { get; set; }
        public Action<Guid, int, IHubContext<GameHub>> OnRoundTimeStoped { get; set; }
        public Action<Guid, int, IHubContext<GameHub>> OnRoundTimeOver { get; set; }

        public Action<Guid, int, IHubContext<GameHub>> OnSendAnswersTimeOver { get; set; }

        public Action<Guid, int, int, string, IHubContext<GameHub>> OnValidationTimeElapsed { get; set; }
        public Action<Guid, int, IHubContext<GameHub>> OnValidationTimeStoped { get; set; }
        public Action<Guid, int, string, IHubContext<GameHub>> OnValidationTimeOver { get; set; }
        public Action<Guid, int, string> OnSendValidationTimeOver { get; set; }

        public void Register(Guid gameId, int roundTime) =>
            _gamesRoundsTimes.Add(gameId, roundTime);

        public void StartRoundTimer(Guid gameId, int roundNumber)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, roundNumber, _gamesRoundsTimes[gameId]);
            Timer roundTimer = new Timer((context) =>
            {
                var roundTimerContext = (TimerContext)context;
                if (roundTimerContext.IsLimitTimeReached())
                {
                    RemoveGameTimer(gameId);
                    OnRoundTimeOver(gameId, roundNumber, _gameHub);
                }
                else
                {
                    OnRoundTimeElapsed(gameId, roundNumber, ++roundTimerContext.ElapsedTime, _gameHub);
                }
            }, gameTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, roundTimer);
        }

        public void StopRoundTimer(Guid gameId, int roundNumber)
        {
            RemoveGameTimer(gameId);
            OnRoundTimeStoped(gameId, roundNumber, _gameHub);
        }

        public void StartSendAnswersTimer(Guid gameId, int roundNumber)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, roundNumber, Consts.SEND_ANSWERS_LIMIT_TIME);
            Timer sendAnswersTimer = new Timer((context) =>
            {
                TimerContext timerContext = (TimerContext)context;
                if (timerContext.IsLimitTimeReached())
                {
                    RemoveGameTimer(gameId);
                    OnSendAnswersTimeOver(gameId, roundNumber, _gameHub);
                }

                timerContext.ElapsedTime++;
            }, gameTimerContext, 1500, 1000);

            AddOrUpdateGameTimer(gameId, sendAnswersTimer);
        }

        public void StartValidationTimer(Guid gameId, int roundNumber, string theme)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, roundNumber, Consts.VALIDATION_LIMIT_TIME);
            Timer validationTimer = new Timer((context) =>
            {
                TimerContext timerContext = (TimerContext)context;
                if (timerContext.IsLimitTimeReached())
                {
                    RemoveGameTimer(gameId);
                    OnValidationTimeOver(gameId, roundNumber, theme, _gameHub);
                }
                else
                {
                    OnValidationTimeElapsed(gameId, roundNumber, ++timerContext.ElapsedTime, theme, _gameHub);
                }
            }, gameTimerContext, 500, 1000);

            AddOrUpdateGameTimer(gameId, validationTimer);
        }

        public void StartSendValidationsTimer(Guid gameId, int roundNumber, string theme)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, roundNumber, Consts.SEND_VALIDATIONS_LIMIT_TIME);
            Timer validationTimer = new Timer((context) =>
            {
                TimerContext timerContext = (TimerContext)context;
                if (timerContext.IsLimitTimeReached())
                {
                    RemoveGameTimer(gameId);
                    OnSendValidationTimeOver(gameId, roundNumber, theme);
                }

                timerContext.ElapsedTime++;
            }, gameTimerContext, 500, 1000);

            AddOrUpdateGameTimer(gameId, validationTimer);
        }

        public void StopValidationTimer(Guid gameId, int roundNumber)
        {
            RemoveGameTimer(gameId);
            OnValidationTimeStoped(gameId, roundNumber, _gameHub);
        }

        private TimerContext CreateGameTimerContext(Guid gameId, int roundNumber, int limitTime) =>
            new TimerContext(gameId, roundNumber, limitTime);

        private void AddOrUpdateGameTimer(Guid gameId, Timer timer)
        {
            if (_timers.ContainsKey(gameId))
            {
                RemoveGameTimer(gameId);
            }

            _timers[gameId] = timer;
        }

        public void RemoveGameTimer(Guid gameId)
        {
            if (_timers.ContainsKey(gameId))
            {
                _timers[gameId]?.Change(Timeout.Infinite, Timeout.Infinite);
                _timers[gameId]?.Dispose();
                _timers[gameId] = null;
            }
        }
    }
}
