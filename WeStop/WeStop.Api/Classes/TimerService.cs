using System;
using System.Collections.Generic;
using System.Threading;
using WeStop.Api.Infra.Services;

namespace WeStop.Api.Classes
{
    public sealed class TimerService : ITimerService
    {
        private const int SEND_ANSWERS_LIMIT_TIME = 3;
        private const int VALIDATION_LIMIT_TIME = 15;
        private static Dictionary<Guid, Timer> _timers = new Dictionary<Guid, Timer>();

        public void StartSendAnswersTime(Guid gameId, Action<Guid, int> onTimeElapsedAction, Action<Guid> onTimeOverAction)
        {
            GameTimerContext gameTimerContext = CreateGameTimerContext(gameId, SEND_ANSWERS_LIMIT_TIME);
            Timer sendAnswersTimer = new Timer((context) =>
            {
                GameTimerContext timerContext = (GameTimerContext)context;
                if (timerContext.ElapsedTime >= timerContext.LimitTime)
                {
                    RemoveGameTimer(gameId);
                    onTimeOverAction?.Invoke(timerContext.GameId);
                }
                else
                {
                    timerContext.AddSecondsToElapsedTime(1);
                    onTimeElapsedAction?.Invoke(timerContext.GameId, timerContext.ElapsedTime);
                }
            }, gameTimerContext, 500, 1000);

            AddOrUpdateGameTimer(gameId, sendAnswersTimer);
        }

        public void StartRoundTimer(Guid gameId, int limitTime, Action<Guid, int> onTimeElapsedAction, Action<Guid> onTimeOverAction)
        {
            GameTimerContext gameTimerContext = CreateGameTimerContext(gameId, limitTime);
            Timer roundTimer = new Timer((context) =>
            {
                GameTimerContext timerContext = (GameTimerContext)context;
                if (timerContext.ElapsedTime >= timerContext.LimitTime)
                {
                    StopRoundTimer(gameId);
                    onTimeOverAction?.Invoke(timerContext.GameId);
                }
                else
                {
                    timerContext.AddSecondsToElapsedTime(1);
                    onTimeElapsedAction?.Invoke(timerContext.GameId, timerContext.ElapsedTime);
                }
            }, gameTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, roundTimer);
        }

        public void StopRoundTimer(Guid gameId)
        {
            RemoveGameTimer(gameId);
        }

        public void StartValidationTimerForTheme(Guid gameId, string themeBeingValidated, Action<Guid, string, int> onTimeElapsedAction, Action<Guid, string> onTimeOverAction)
        {
            ThemeValidationTimerContext themeValidationTimerContext = CreateThemeValidationTimerContext(gameId, themeBeingValidated, VALIDATION_LIMIT_TIME);
            Timer validationTimer = new Timer((context) =>
            {
                ThemeValidationTimerContext timerContext = (ThemeValidationTimerContext)context;
                if (timerContext.ElapsedTime >= timerContext.LimitTime)
                {
                    StopValidationTimer(gameId);
                    onTimeOverAction?.Invoke(timerContext.GameId, timerContext.ThemeBeingValidated);
                }
                else
                {
                    timerContext.AddSecondsToElapsedTime(1);
                    onTimeElapsedAction(timerContext.GameId, timerContext.ThemeBeingValidated, timerContext.ElapsedTime);
                }
            }, themeValidationTimerContext, 1000, 1000);

            AddOrUpdateGameTimer(gameId, validationTimer);
        }

        public void StopValidationTimer(Guid gameId)
        {
            RemoveGameTimer(gameId);
        }

        private ThemeValidationTimerContext CreateThemeValidationTimerContext(Guid gameId, string themeBeingValidated, int limitTime) =>
            new ThemeValidationTimerContext(gameId, themeBeingValidated, limitTime);

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
