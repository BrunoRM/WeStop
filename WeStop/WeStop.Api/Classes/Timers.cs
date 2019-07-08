using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading;
using WeStop.Api.Infra.Hubs;

namespace WeStop.Api.Classes
{
    public sealed class Timers
    {
        private const int SEND_ANSWERS_LIMIT_TIME = 3;
        private const int VALIDATION_LIMIT_TIME = 10;
        private readonly IHubContext<GameHub> _hubContext;
        private static Dictionary<Guid, Timer> _timers = new Dictionary<Guid, Timer>();

        public Timers(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void StartSendAnswersTime(Guid gameId, Action<Guid, IHubContext<GameHub>, int> onTimeElapsedAction, Action<Guid, IHubContext<GameHub>> onTimeOverAction)
        {
            GameTimerContext gameTimerContext = CreateGameTimerContext(gameId, SEND_ANSWERS_LIMIT_TIME);
            Timer sendAnswersTimer = new Timer((context) =>
            {
                GameTimerContext timerContext = (GameTimerContext)context;
                if (timerContext.ElapsedTime >= timerContext.LimitTime)
                {
                    RemoveGameTimer(gameId);
                    onTimeOverAction?.Invoke(timerContext.GameId, _hubContext);
                }
                else
                {
                    timerContext.AddSecondsToElapsedTime(1);
                    onTimeElapsedAction?.Invoke(timerContext.GameId, _hubContext, timerContext.ElapsedTime);
                }
            }, gameTimerContext, 500, 1000);

            AddOrUpdateGameTimer(gameId, sendAnswersTimer);
        }

        public void StartRoundTimer(Guid gameId, int limitTime, Action<Guid, IHubContext<GameHub>, int> onTimeElapsedAction, Action<Guid, IHubContext<GameHub>> onTimeOverAction)
        {
            GameTimerContext gameTimerContext = CreateGameTimerContext(gameId, limitTime);
            Timer roundTimer = new Timer((context) =>
            {
                GameTimerContext timerContext = (GameTimerContext)context;
                if (timerContext.ElapsedTime >= timerContext.LimitTime)
                {
                    StopRoundTimer(gameId);
                    onTimeOverAction?.Invoke(timerContext.GameId, _hubContext);
                }
                else
                {
                    timerContext.AddSecondsToElapsedTime(1);
                    onTimeElapsedAction?.Invoke(timerContext.GameId, _hubContext, timerContext.ElapsedTime);
                }
            }, gameTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, roundTimer);
        }

        public void StopRoundTimer(Guid gameId)
        {
            RemoveGameTimer(gameId);
        }

        public void StartValidationTimer(Guid gameId, Action<Guid, IHubContext<GameHub>, int> onTimeElapsedAction, Action<Guid, IHubContext<GameHub>> onTimeOverAction)
        {
            GameTimerContext gameTimerContext = CreateGameTimerContext(gameId, VALIDATION_LIMIT_TIME);
            Timer validationTimer = new Timer((context) =>
            {
                GameTimerContext timerContext = (GameTimerContext)context;
                if (timerContext.ElapsedTime >= timerContext.LimitTime)
                {
                    StopValidationTimer(gameId);
                    onTimeOverAction?.Invoke(timerContext.GameId, _hubContext);
                }
                else
                {
                    timerContext.AddSecondsToElapsedTime(1);
                    onTimeElapsedAction(timerContext.GameId, _hubContext, timerContext.ElapsedTime);
                }
            }, gameTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, validationTimer);
        }

        public void StopValidationTimer(Guid gameId)
        {
            RemoveGameTimer(gameId);
        }

        private GameTimerContext CreateGameTimerContext(Guid gameId, int limitTime) =>
            new GameTimerContext(gameId, limitTime);

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
