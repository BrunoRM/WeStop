using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading;
using WeStop.Api.Infra.Hubs;
using WeStop.Api.Infra.Timers.Interfaces;

namespace WeStop.Api.Infra.Timers.Default
{
    public sealed class GameTimer : IGameTimer
    {
        private const int SEND_ANSWERS_LIMIT_TIME = 3;
        private const int VALIDATION_LIMIT_TIME = 45;
        private static readonly Dictionary<Guid, Timer> _timers = new Dictionary<Guid, Timer>();
        private readonly IHubContext<GameHub> _gameHub;

        public GameTimer(IHubContext<GameHub> gameHub)
        {
            _gameHub = gameHub;
        }

        public void StartSendAnswersTimer(Guid gameId, Action<Guid, int, IHubContext<GameHub>> onTimeElapsedAction, Action<Guid, IHubContext<GameHub>> onTimeOverAction)
        {
            GameTimerContext gameTimerContext = CreateGameTimerContext(gameId, SEND_ANSWERS_LIMIT_TIME);
            Timer sendAnswersTimer = new Timer((context) =>
            {
                GameTimerContext timerContext = (GameTimerContext)context;
                if (timerContext.ElapsedTime >= timerContext.RoundTime)
                {
                    RemoveGameTimer(gameId);
                    onTimeOverAction?.Invoke(timerContext.GameId, _gameHub);
                }
                else
                {
                    timerContext.AddSecondsToElapsedTime(1);
                    onTimeElapsedAction?.Invoke(timerContext.GameId, timerContext.ElapsedTime, _gameHub);
                }
            }, gameTimerContext, 500, 1000);

            AddOrUpdateGameTimer(gameId, sendAnswersTimer);
        }

        public void StartRoundTimer(Guid gameId, int limitTime, Action<Guid, int, IHubContext<GameHub>> onTimeElapsedAction, Action<Guid, IHubContext<GameHub>> onTimeOverAction)
        {
            GameTimerContext gameTimerContext = CreateGameTimerContext(gameId, limitTime);
            Timer roundTimer = new Timer((context) =>
            {
                GameTimerContext timerContext = (GameTimerContext)context;
                if (timerContext.ElapsedTime >= timerContext.RoundTime)
                {
                    StopRoundTimer(gameId);
                    onTimeOverAction?.Invoke(timerContext.GameId, _gameHub);
                }
                else
                {
                    timerContext.AddSecondsToElapsedTime(1);
                    onTimeElapsedAction?.Invoke(timerContext.GameId, timerContext.ElapsedTime, _gameHub);
                }
            }, gameTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, roundTimer);
        }

        public void StopRoundTimer(Guid gameId)
        {
            RemoveGameTimer(gameId);
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

        public void StartValidationTimer(Guid gameId, Action<Guid, int, IHubContext<GameHub>> onTimeElapsed, Action<Guid, IHubContext<GameHub>> onTimeOver)
        {
            GameTimerContext gameTimerContext = CreateGameTimerContext(gameId, VALIDATION_LIMIT_TIME);
            Timer validationTimer = new Timer((context) =>
            {
                GameTimerContext timerContext = (GameTimerContext)context;
                if (timerContext.ElapsedTime >= VALIDATION_LIMIT_TIME)
                {
                    onTimeOver?.Invoke(timerContext.GameId, _gameHub);
                }
                else
                {
                    timerContext.AddSecondsToElapsedTime(1);
                    onTimeElapsed?.Invoke(timerContext.GameId, timerContext.ElapsedTime, _gameHub);
                }
            }, gameTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, validationTimer);
        }
    }
}
