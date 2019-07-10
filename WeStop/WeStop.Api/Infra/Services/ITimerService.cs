using System;

namespace WeStop.Api.Infra.Services
{
    public interface ITimerService
    {
        void StartSendAnswersTime(Guid gameId, Action<Guid, int> onTimeElapsedAction, Action<Guid> onTimeOverAction);
        void StartRoundTimer(Guid gameId, int limitTime, Action<Guid, int> onTimeElapsedAction, Action<Guid> onTimeOverAction);
        void StopRoundTimer(Guid gameId);
        void StartValidationTimerForTheme(Guid gameId, string themeBeingValidated, Action<Guid, string, int> onTimeElapsedAction, Action<Guid, string> onTimeOverAction);
        void StopValidationTimer(Guid gameId);
    }
}
