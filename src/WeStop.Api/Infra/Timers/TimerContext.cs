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

        public TimerContext(Guid gameId, int limitTime, string themeBeingValidated)
            :this(gameId, limitTime)
        {            
            ThemeBeingValidated = themeBeingValidated;
        }

        public Guid GameId { get; private set; }
        public int LimitTime { get; private set; }
        public int ElapsedTime { get; set; }
        public string ThemeBeingValidated { get; set; }

        public bool IsLimitTimeReached() =>
            ElapsedTime >= LimitTime;
    }
}
