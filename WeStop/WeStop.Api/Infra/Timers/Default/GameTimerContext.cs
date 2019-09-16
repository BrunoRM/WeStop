using System;

namespace WeStop.Api.Infra.Timers.Default
{
    public class GameTimerContext
    {
        public GameTimerContext(Guid gameId, int roundTime)
        {
            GameId = gameId;
            RoundTime = roundTime;
        }

        public Guid GameId { get; private set; }
        public int RoundTime { get; private set; }
        public int ElapsedTime { get; private set; }

        public void AddSecondsToElapsedTime(int seconds)
        {
            ElapsedTime += seconds;
        }
    }
}
