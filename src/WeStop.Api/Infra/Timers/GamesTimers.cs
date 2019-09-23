using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading;
using WeStop.Api.Infra.Hubs;

namespace WeStop.Api.Infra.Timers
{
    public sealed class GamesTimers
    {
        private static readonly IDictionary<Guid, int> _gamesRoundsTimes = new Dictionary<Guid, int>();
        private static readonly Dictionary<Guid, Timer> _timers = new Dictionary<Guid, Timer>();
        private readonly IHubContext<GameHub> _gameHub;

        public GamesTimers(IHubContext<GameHub> gameHub)
        {
            _gameHub = gameHub;
        }

        public void Register(Guid gameId, int roundTime) =>
            _gamesRoundsTimes.Add(gameId, roundTime);
        
        public void StartRoundTimer(Guid gameId, int roundNumber, Action<Guid, int, IHubContext<GameHub>> onTimeElapsedAction, Action<Guid, IHubContext<GameHub>> onTimeOverAction)
        {
            var limitTime = _gamesRoundsTimes[gameId];
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, roundNumber, limitTime);
            Timer roundTimer = new Timer((context) =>
            {
                TimerContext roundTimerContext = (TimerContext)context;
                if (roundTimerContext.ElapsedTime >= roundTimerContext.LimitTime)
                {
                    StopRoundTimer(gameId);
                    onTimeOverAction?.Invoke(roundTimerContext.GameId, _gameHub);
                }
                else
                {
                    roundTimerContext.ElapsedTime += 1;
                    onTimeElapsedAction?.Invoke(roundTimerContext.GameId, roundTimerContext.ElapsedTime, _gameHub);
                }
            }, gameTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, roundTimer);
        }

        public void StopRoundTimer(Guid gameId)
        {
            RemoveGameTimer(gameId);
        }

        public void StartSendAnswersTimer(Guid gameId, int roundNumber, Action<Guid, int, IHubContext<GameHub>> onTimeElapsedAction, Action<Guid, IHubContext<GameHub>> onTimeOverAction)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, roundNumber, Consts.SEND_ANSWERS_LIMIT_TIME);
            Timer sendAnswersTimer = new Timer((context) =>
            {
                TimerContext timerContext = (TimerContext)context;
                if (timerContext.ElapsedTime >= timerContext.LimitTime)
                {
                    RemoveGameTimer(gameId);
                    onTimeOverAction?.Invoke(timerContext.GameId, _gameHub);
                }
                else
                {
                    timerContext.ElapsedTime += 1;
                    onTimeElapsedAction?.Invoke(timerContext.GameId, timerContext.ElapsedTime, _gameHub);
                }
            }, gameTimerContext, 500, 1000);

            AddOrUpdateGameTimer(gameId, sendAnswersTimer);
        }
        
        public void StartValidationTimer(Guid gameId, int roundNumber, Action<Guid, int, IHubContext<GameHub>> onTimeElapsed, Action<Guid, IHubContext<GameHub>> onTimeOver)
        {
            TimerContext gameTimerContext = CreateGameTimerContext(gameId, roundNumber, Consts.VALIDATION_LIMIT_TIME);
            Timer validationTimer = new Timer((context) =>
            {
                TimerContext timerContext = (TimerContext)context;
                if (timerContext.ElapsedTime >= Consts.VALIDATION_LIMIT_TIME)
                {
                    onTimeOver?.Invoke(timerContext.GameId, _gameHub);
                }
                else
                {
                    timerContext.ElapsedTime += 1;
                    onTimeElapsed?.Invoke(timerContext.GameId, timerContext.ElapsedTime, _gameHub);
                }
            }, gameTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, validationTimer);
        }

        public void StopValidationTimer(Guid gameId)
        {
            RemoveGameTimer(gameId);
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
