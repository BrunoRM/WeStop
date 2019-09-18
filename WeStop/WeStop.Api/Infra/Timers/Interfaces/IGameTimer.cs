using Microsoft.AspNetCore.SignalR;
using System;
using WeStop.Api.Infra.Hubs;

namespace WeStop.Api.Infra.Timers.Interfaces
{
    public interface IGameTimer
    {
        void StartSendAnswersTimer(Guid gameId, Action<Guid, int, IHubContext<GameHub>> onTimeElapsedAction, Action<Guid, IHubContext<GameHub>> onTimeOverAction);
        void StartRoundTimer(Guid gameId, int limitTime, Action<Guid, int, IHubContext<GameHub>> onTimeElapsedAction, Action<Guid, IHubContext<GameHub>> onTimeOverAction);
        void StopRoundTimer(Guid gameId);
        void StartValidationTimer(Guid gameId, Action<Guid, int, IHubContext<GameHub>> onTimeElapsed, Action<Guid, IHubContext<GameHub>> onTimeOver);
        void StopValidationTimer(Guid gameId);
    }
}
