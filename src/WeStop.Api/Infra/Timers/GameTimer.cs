﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Api.Domain.Services;
using WeStop.Api.Infra.Hubs;
using WeStop.Api.Managers;

namespace WeStop.Api.Infra.Timers
{
    public sealed class GameTimer
    {
        private static readonly IDictionary<Guid, int> _gamesRoundsTimes = new Dictionary<Guid, int>();
        private static readonly ConcurrentDictionary<Guid, Timer> _timers = new ConcurrentDictionary<Guid, Timer>();
        private readonly IHubContext<GameHub> _gameHub;
        private readonly GameManager _gameManager;
        private readonly RoundScorer _roundScorer;

        public GameTimer(IHubContext<GameHub> gameHub, GameManager gameManager, RoundScorer roundScorer)
        {
            _gameHub = gameHub;
            _gameManager = gameManager;
            _roundScorer = roundScorer;

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
                await hub.Clients.Group(gameId.ToString()).SendAsync("round_stop", new
                {
                    reason = "time_over"
                });

                StartSendAnswersTimer(gameId, currentRoundNumber);
            };

            OnSendAnswersTimeOver += async (gameId, currentRoundNumber, hub) =>
            {
                await StartValidationForNextTheme(gameId, currentRoundNumber, hub);
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
                await StartValidationForNextTheme(gameId, roundNumber, hub);
            };
        }

        private async Task StartValidationForNextTheme(Guid gameId, int currentRoundNumber, IHubContext<GameHub> hub)
        {
            await _gameManager.StartValidationForNextThemeAsync(gameId, currentRoundNumber, async (theme) =>
            {
                // TODO: Remover duplicidade de código daqui e do hub
                var gameConnectionsIds = ConnectionBinding.GetGameConnections(gameId);

                var playersValidations = await _gameManager.GetPlayersDefaultValidationsAsync(gameId, theme);

                foreach (var (playerId, validations) in playersValidations)
                {
                    if (gameConnectionsIds.Any(gc => gc.PlayerId == playerId))
                    {
                        string connectionId = gameConnectionsIds.First(gc => gc.PlayerId == playerId).ConnectionId;
                        await hub.Clients.Client(connectionId).SendAsync("validation_started", new
                        {
                            theme,
                            validations
                        });
                    }
                }

                StartValidationTimer(gameId, currentRoundNumber, theme);
            }, null);
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
        
        public void StartValidationTimer(Guid gameId, int roundNumber, string theme)
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
            }, gameTimerContext, 1500, 1000);

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
