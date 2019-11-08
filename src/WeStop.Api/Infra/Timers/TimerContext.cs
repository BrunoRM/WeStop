using System;

namespace WeStop.Api.Infra.Timers
{
    public class TimerContext
    {
        public TimerContext(Guid gameId, int roundNumber, int limitTime)
        {
            GameId = gameId;
            RoundNumber = roundNumber;
            LimitTime = limitTime;
            ElapsedTime = 0;
        }

        public TimerContext(Guid gameId, int roundNumber, int limitTime, string themeBeingValidated)
            :this(gameId, roundNumber, limitTime)
        {            
            ThemeBeingValidated = themeBeingValidated;
        }

        public Guid GameId { get; private set; }
        public int RoundNumber { get; set; }
        public int LimitTime { get; private set; }
        public int ElapsedTime { get; set; }
        public string ThemeBeingValidated { get; set; }

        public bool IsLimitTimeReached() =>
            ElapsedTime >= LimitTime;
    }
}
