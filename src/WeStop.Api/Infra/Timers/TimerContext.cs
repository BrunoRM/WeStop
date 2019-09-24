using System;

namespace WeStop.Api.Infra.Timers
{
    public struct TimerContext
    {
        public TimerContext(Guid gameId, int roundNumber, int limitTime)
        {
            GameId = gameId;
            RoundNumber = roundNumber;
            LimitTime = limitTime;
            ElapsedTime = 0;
        }

        public Guid GameId { get; private set; }
        public int RoundNumber { get; private set; }
        public int LimitTime { get; private set; }
        public int ElapsedTime { get; set; }
    }
}
