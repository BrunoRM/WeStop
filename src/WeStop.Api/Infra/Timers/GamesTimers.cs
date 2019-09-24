﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WeStop.Api.Domain.Services;
using WeStop.Api.Infra.Hubs;
using WeStop.Api.Managers;

namespace WeStop.Api.Infra.Timers
{
    public sealed class GamesTimers
    {
        private static readonly IDictionary<Guid, int> _gamesRoundsTimes = new Dictionary<Guid, int>();
        private static readonly Dictionary<Guid, Timer> _timers = new Dictionary<Guid, Timer>();
        private readonly IHubContext<GameHub> _gameHub;
        private readonly GameManager _gameManager;
        private readonly RoundScorer _roundScorer;

        public GamesTimers(IHubContext<GameHub> gameHub, GameManager gameManager, RoundScorer roundScorer)
        {
            _gameHub = gameHub;
            _gameManager = gameManager;
            _roundScorer = roundScorer;

            OnRoundTimeElapsed += async (gameId, currentTime, hub) =>
            {
                await hub.Clients.Group(gameId.ToString()).SendAsync("round_time_elapsed", currentTime);
            };

            OnRoundTimeOver += async (gameId, roundNumber, hub) =>
            {
                await hub.Clients.Group(gameId.ToString()).SendAsync("round_stop", new
                {
                    reason = "time_over"
                });
            };

            OnSendAnswersTimeOver += async (gameId, roundNumber, hub) =>
            {
                await hub.Clients.Group(gameId.ToString()).SendAsync("send_answers_time_over");

                var gameConnectionsIds = ConnectionBinding.GetGameConnections(gameId);

                var playersValidations = await _gameManager.GetPlayersDefaultValidationsAsync(gameId, roundNumber);

                foreach (var (playerId, validations) in playersValidations)
                {
                    if (gameConnectionsIds.Any(gc => gc.PlayerId == playerId))
                    {
                        string connectionId = gameConnectionsIds.First(gc => gc.PlayerId == playerId).ConnectionId;
                        await hub.Clients.Client(connectionId).SendAsync("validation_started", validations);
                    }
                }

                StartValidationTimer(gameId, roundNumber);
            };

            OnValidationTimeElapsed += async (gameId, currentTime, hubContext) =>
            {
                await hubContext.Clients.Group(gameId.ToString()).SendAsync("validation_time_elapsed", new
                {
                    currentTime
                });
            };

            OnRoundTimeOver += async (gameId, roundNumber, hubContext) =>
            {
                await _gameManager.FinishCurrentRoundAsync(gameId);

                var roundPontuation = await _roundScorer.ProcessCurrentRoundPontuationAsync(gameId);
                await hubContext.Clients.Group(gameId.ToString()).SendAsync("round_finished", new
                {
                    scoreboard = roundPontuation
                });
            };
        }

        public Action<Guid, int, IHubContext<GameHub>> OnRoundTimeElapsed { get; set; }
        public Action<Guid, IHubContext<GameHub>> OnRoundTimeStoped { get; set; }
        public Action<Guid, int, IHubContext<GameHub>> OnRoundTimeOver { get; set; }

        public Action<Guid, int, IHubContext<GameHub>> OnSendAnswersTimeElapsed { get; set; }
        public Action<Guid, int, IHubContext<GameHub>> OnSendAnswersTimeOver { get; set; }

        public Action<Guid, int, IHubContext<GameHub>> OnValidationTimeElapsed { get; set; }
        public Action<Guid, IHubContext<GameHub>> OnValidationTimeStoped { get; set; }
        public Action<Guid, int, IHubContext<GameHub>> OnValidationTimeOver { get; set; }

        public void Register(Guid gameId, int roundTime) =>
            _gamesRoundsTimes.Add(gameId, roundTime);
        
        public void StartRoundTimer(Guid gameId, int roundNumber)
        {
            var limitTime = _gamesRoundsTimes[gameId];
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, roundNumber, limitTime);
            Timer roundTimer = new Timer((context) =>
            {
                TimerContext roundTimerContext = (TimerContext)context;
                if (roundTimerContext.ElapsedTime >= roundTimerContext.LimitTime)
                {
                    StopRoundTimer(gameId);
                    OnRoundTimeOver(gameId, roundTimerContext.RoundNumber, _gameHub);
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
                else
                {
                    OnSendAnswersTimeElapsed(gameId, ++timerContext.ElapsedTime, _gameHub);
                }
            }, gameTimerContext, 500, 1000);

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
                    OnValidationTimeOver(gameId, timerContext.RoundNumber, _gameHub);
                }
                else
                {
                    OnValidationTimeElapsed(gameId, ++timerContext.ElapsedTime, _gameHub);
                }
            }, gameTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, validationTimer);
        }

        public void StopValidationTimer(Guid gameId)
        {
            RemoveGameTimer(gameId);
            OnValidationTimeStoped(gameId, _gameHub);
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
