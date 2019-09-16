using System;

namespace WeStop.Api.Infra.Timers.Interfaces
{
    public interface IGameTimer
    {
        void StartSendAnswersTime(Guid gameId, Action<Guid, int> onTimeElapsedAction, Action<Guid> onTimeOverAction);
        void StartRoundTimer(Guid gameId, int limitTime, Action<Guid, int> onTimeElapsedAction, Action<Guid> onTimeOverAction);
        void StopRoundTimer(Guid gameId);
        void StartValidationTimer(Guid gameId, Action<Guid, int> onTimeElapsed, Action<Guid> onTimeOver);
        void StopValidationTimer(Guid gameId);
    }
}
