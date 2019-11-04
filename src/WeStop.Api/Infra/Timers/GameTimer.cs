using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IHubContext<LobbyHub> _lobbyHubContext;

        public GameTimer(IHubContext<GameHub> gameHub, GameManager gameManager,
            IHubContext<LobbyHub> lobbyHubContext)
        {
            _gameHub = gameHub;
            _gameManager = gameManager;
            _lobbyHubContext = lobbyHubContext;

            OnRoundTimeElapsed += async (gameId, currentTime, hub) =>
            {
                await hub.Clients.Group(gameId.ToString()).SendAsync("round_time_elapsed", currentTime);
            };

            OnRoundTimeStoped += (gameId, hub) =>
            {
                StartSendAnswersTimer(gameId);
            };

            OnRoundTimeOver += async (gameId, hub) =>
            {
                await hub.Clients.Group(gameId.ToString()).SendAsync("round_stoped", new
                {
                    reason = "time_over"
                });

                StartSendAnswersTimer(gameId);
            };

            OnSendAnswersTimeOver += async (gameId, hub) =>
            {
                await StartValidationForNextThemeAsync(gameId);
            };

            OnValidationTimeElapsed += async (gameId, currentTime, theme, hub) =>
            {
                await hub.Clients.Group(gameId.ToString()).SendAsync("validation_time_elapsed", new
                {
                    currentTime
                });
            };

            OnValidationTimeOver += async (gameId, theme, hub) =>
            {
                var inRoundPlayersIds = await _gameManager.GetInRoundPlayersIdsAsync(gameId);

                foreach (var playerId in inRoundPlayersIds)
                {
                    var playerConnectionId = ConnectionBinding.GetPlayerConnectionId(gameId, playerId);
                    await hub.Clients.Client(playerConnectionId).SendAsync("validation_time_over");
                }

                StartSendValidationsTimer(gameId, theme);
            };

            OnSendValidationTimeOver += async (gameId, theme) =>
            {
                await _gameManager.FinishValidationsForThemeAsync(gameId, theme);
                await StartValidationForNextThemeAsync(gameId);
            };
        }

        public async Task StartValidationForNextThemeAsync(Guid gameId)
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

                StartValidationTimer(gameId, themeToValidate);
            }
            else
            {
                await _gameManager.FinishCurrentRoundAsync(gameId, async (game) =>
                {
                    var roundScoreboard = game.GetScoreboard(game.CurrentRoundNumber);
                    if (game.IsFinalRound())
                    {
                        var winners = game.GetWinners();
                        await _gameHub.Clients.Group(gameId.ToString()).SendAsync("game_finished", new
                        {
                            lastRoundScoreboard = roundScoreboard,
                            winners
                        });

                        await _lobbyHubContext.Clients.All.SendAsync("game_finished", gameId);
                    }
                    else
                    {
                        await _gameHub.Clients.Group(gameId.ToString()).SendAsync("round_finished", new
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

        public Action<Guid, int, IHubContext<GameHub>> OnRoundTimeElapsed { get; set; }
        public Action<Guid, IHubContext<GameHub>> OnRoundTimeStoped { get; set; }
        public Action<Guid, IHubContext<GameHub>> OnRoundTimeOver { get; set; }

        public Action<Guid, IHubContext<GameHub>> OnSendAnswersTimeOver { get; set; }

        public Action<Guid, int, string, IHubContext<GameHub>> OnValidationTimeElapsed { get; set; }
        public Action<Guid, IHubContext<GameHub>> OnValidationTimeStoped { get; set; }
        public Action<Guid, string, IHubContext<GameHub>> OnValidationTimeOver { get; set; }
        public Action<Guid, string> OnSendValidationTimeOver { get; set; }

        public void Register(Guid gameId, int roundTime) =>
            _gamesRoundsTimes.Add(gameId, roundTime);

        public void StartRoundTimer(Guid gameId)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, _gamesRoundsTimes[gameId]);
            Timer roundTimer = new Timer((context) =>
            {
                var roundTimerContext = (TimerContext)context;
                if (roundTimerContext.IsLimitTimeReached())
                {
                    RemoveGameTimer(gameId);
                    OnRoundTimeOver(gameId, _gameHub);
                }
                else
                {
                    OnRoundTimeElapsed(gameId, ++roundTimerContext.ElapsedTime, _gameHub);
                }
            }, gameTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, roundTimer);
        }

        public void StopRoundTimer(Guid gameId)
        {
            RemoveGameTimer(gameId);
            OnRoundTimeStoped(gameId, _gameHub);
        }

        public void StartSendAnswersTimer(Guid gameId)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, Consts.SEND_ANSWERS_LIMIT_TIME);
            Timer sendAnswersTimer = new Timer((context) =>
            {
                TimerContext timerContext = (TimerContext)context;
                if (timerContext.IsLimitTimeReached())
                {
                    RemoveGameTimer(gameId);
                    OnSendAnswersTimeOver(gameId, _gameHub);
                }

                timerContext.ElapsedTime++;
            }, gameTimerContext, 1500, 1000);

            AddOrUpdateGameTimer(gameId, sendAnswersTimer);
        }

        public void StartValidationTimer(Guid gameId, string theme)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, Consts.VALIDATION_LIMIT_TIME);
            Timer validationTimer = new Timer((context) =>
            {
                TimerContext timerContext = (TimerContext)context;
                if (timerContext.IsLimitTimeReached())
                {
                    RemoveGameTimer(gameId);
                    OnValidationTimeOver(gameId, theme, _gameHub);
                }
                else
                {
                    OnValidationTimeElapsed(gameId, ++timerContext.ElapsedTime, theme, _gameHub);
                }
            }, gameTimerContext, 500, 1000);

            AddOrUpdateGameTimer(gameId, validationTimer);
        }

        public void StartSendValidationsTimer(Guid gameId, string theme)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, Consts.SEND_VALIDATIONS_LIMIT_TIME);
            Timer validationTimer = new Timer((context) =>
            {
                TimerContext timerContext = (TimerContext)context;
                if (timerContext.IsLimitTimeReached())
                {
                    RemoveGameTimer(gameId);
                    OnSendValidationTimeOver(gameId, theme);
                }

                timerContext.ElapsedTime++;
            }, gameTimerContext, 500, 1000);

            AddOrUpdateGameTimer(gameId, validationTimer);
        }

        public void StopValidationTimer(Guid gameId)
        {
            RemoveGameTimer(gameId);
            OnValidationTimeStoped(gameId, _gameHub);
        }

        private TimerContext CreateGameTimerContext(Guid gameId, int limitTime) =>
            new TimerContext(gameId, limitTime);

        private void AddOrUpdateGameTimer(Guid gameId, Timer timer)
        {
            if (_timers.ContainsKey(gameId))
            {
                RemoveGameTimer(gameId);
            }

            _timers[gameId] = timer;
        }

        private void RemoveGameTimer(Guid gameId)
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
