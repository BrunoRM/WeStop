using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Api.Core.Services;
using WeStop.Api.Infra.Hubs;

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

            OnRoundTimeElapsed += async (gameId, currentRoundNumber, currentTime, hub) =>
            {
                await hub.Clients.Group(gameId.ToString()).SendAsync("round_time_elapsed", currentTime);
            };

            OnRoundTimeStoped += (gameId, currentRoundNumber, hub) =>
            {
                StartSendAnswersTimer(gameId, currentRoundNumber);
            };

            OnRoundTimeOver += async (gameId, currentRoundNumber, hub) =>
            {
                await hub.Clients.Group(gameId.ToString()).SendAsync("round_stoped", new
                {
                    reason = "time_over"
                });

                StartSendAnswersTimer(gameId, currentRoundNumber);
            };

            OnSendAnswersTimeOver += async (gameId, currentRoundNumber, hub) =>
            {
                await StartValidationForNextThemeAsync(gameId, currentRoundNumber);
            };

            OnValidationTimeElapsed += async (gameId, roundNumber, currentTime, hub) =>
            {
                await hub.Clients.Group(gameId.ToString()).SendAsync("validation_time_elapsed", new
                {
                    currentTime
                });
            };

            OnValidationTimeOver += async (gameId, roundNumber, hub) =>
            {
                var inRoundPlayersIds = await _gameManager.GetInRoundPlayersIdsAsync(gameId);

                foreach (var playerId in inRoundPlayersIds)
                {
                    var playerConnectionId = ConnectionBinding.GetPlayerConnectionId(gameId, playerId);
                    await hub.Clients.Client(playerConnectionId).SendAsync("validation_time_over");
                }
            };
        }

        public Task StartValidationForNextThemeAsync(Guid gameId, int currentRoundNumber)
        {
            return Task.Run(() =>
            {
                StartValidationForNextTheme(gameId, currentRoundNumber);
            });
        }

        public void StartValidationForNextTheme(Guid gameId, int currentRoundNumber)
        {
            var themeToValidate = _gameManager.StartValidationForNextTheme(gameId);

            if (!string.IsNullOrEmpty(themeToValidate))
            {
                var playersValidations = _gameManager.GetPlayersDefaultValidations(gameId, themeToValidate);

                foreach (var (playerId, validations, totalValidations, validationsNumber) in playersValidations)
                {
                    string connectionId = ConnectionBinding.GetPlayerConnectionId(gameId, playerId);

                    _gameHub.Clients.Client(connectionId).SendAsync("validation_started", new
                    {
                        theme = themeToValidate,
                        validations,
                        totalValidations,
                        validationsNumber
                    });
                }

                StartValidationTimer(gameId, currentRoundNumber);
            }
            else
            {
                _gameManager.FinishCurrentRound(gameId, (game) =>
                {
                    var roundScoreboard = game.GetScoreboard(game.CurrentRoundNumber);
                    if (game.IsFinalRound())
                    {
                        var winners = game.GetWinners();
                        _gameHub.Clients.Group(gameId.ToString()).SendAsync("game_finished", new
                        {
                            lastRoundScoreboard = roundScoreboard,
                            winners
                        });
                    }
                    else
                    {
                        _gameHub.Clients.Group(gameId.ToString()).SendAsync("round_finished", new
                        {
                            scoreboard = roundScoreboard
                        });
                    }
                });
            }
        }

        public Action<Guid, int, int, IHubContext<GameHub>> OnRoundTimeElapsed { get; set; }
        public Action<Guid, int, IHubContext<GameHub>> OnRoundTimeStoped { get; set; }
        public Action<Guid, int, IHubContext<GameHub>> OnRoundTimeOver { get; set; }

        public Action<Guid, int, IHubContext<GameHub>> OnSendAnswersTimeOver { get; set; }

        public Action<Guid, int, int, IHubContext<GameHub>> OnValidationTimeElapsed { get; set; }
        public Action<Guid, int, IHubContext<GameHub>> OnValidationTimeStoped { get; set; }
        public Action<Guid, int, IHubContext<GameHub>> OnValidationTimeOver { get; set; }

        public void Register(Guid gameId, int roundTime) =>
            _gamesRoundsTimes.Add(gameId, roundTime);

        public void StartRoundTimer(Guid gameId, int roundNumber)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, roundNumber, _gamesRoundsTimes[gameId]);
            Timer roundTimer = new Timer((context) =>
            {
                var roundTimerContext = (TimerContext)context;
                if (roundTimerContext.ElapsedTime >= roundTimerContext.LimitTime)
                {
                    RemoveGameTimer(gameId);
                    OnRoundTimeOver(gameId, roundTimerContext.RoundNumber, _gameHub);
                }
                else
                {
                    OnRoundTimeElapsed(gameId, roundTimerContext.RoundNumber, ++roundTimerContext.ElapsedTime, _gameHub);
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
                if (timerContext.ElapsedTime >= timerContext.LimitTime)
                {
                    RemoveGameTimer(gameId);
                    OnSendAnswersTimeOver(gameId, timerContext.RoundNumber, _gameHub);
                }

                timerContext.ElapsedTime++;
            }, gameTimerContext, 1500, 1000);

            AddOrUpdateGameTimer(gameId, sendAnswersTimer);
        }

        public void StartValidationTimer(Guid gameId, int roundNumber)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, roundNumber, Consts.VALIDATION_LIMIT_TIME);
            Timer validationTimer = new Timer((context) =>
            {
                TimerContext timerContext = (TimerContext)context;
                if (timerContext.ElapsedTime >= Consts.VALIDATION_LIMIT_TIME)
                {
                    RemoveGameTimer(gameId);
                    OnValidationTimeOver(gameId, timerContext.RoundNumber, _gameHub);
                }
                else
                {
                    OnValidationTimeElapsed(gameId, timerContext.RoundNumber, ++timerContext.ElapsedTime, _gameHub);
                }
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
