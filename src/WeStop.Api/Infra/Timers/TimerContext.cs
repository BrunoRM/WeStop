using System;

namespace WeStop.Api.Infra.Timers
{
    public class TimerContext
    {
        public TimerContext(Guid gameId, int limitTime)
        {
            GameId = gameId;
            LimitTime = limitTime;
            ElapsedTime = 0;
        }

        public Guid GameId { get; private set; }
        public int LimitTime { get; private set; }
        public int ElapsedTime { get; set; }
    }
}
