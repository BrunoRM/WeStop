using System;

namespace WeStop.Api.Infra.Timers.Default
{
    public class GameTimerContext
    {
        public GameTimerContext(Guid gameId, int limitTime)
        {
            GameId = gameId;
            LimitTime = limitTime;
        }

        public Guid GameId { get; private set; }
        public int LimitTime { get; private set; }
        public int ElapsedTime { get; private set; }

        public void AddSecondsToElapsedTime(int seconds)
        {
            ElapsedTime += seconds;
        }
    }
}
