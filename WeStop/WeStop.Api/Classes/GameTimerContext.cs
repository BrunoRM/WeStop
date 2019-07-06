using System;

namespace WeStop.Api.Classes
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
            if (seconds < 0)
            {
                throw new Exception("O valor atribuido para o tempo atual não pode ser menor que zero");
            }

            if (seconds > LimitTime)
            {
                throw new Exception("O valor atribuido para o tempo atual não pode ser maior que o tempo limite");
            }

            ElapsedTime += seconds;
        }
    }
}
