using System;
using System.Collections.Generic;

namespace WeStop.Api.Classes
{
    public class RoundAnswers
    {
        public RoundAnswers(Guid gameId, int roundNumber, ICollection<ThemeAnswer> answers)
        {
            GameId = gameId;
            RoundNumber = roundNumber;
            Answers = answers;
        }

        public Guid PlayerId { get; private set; }
        public Guid GameId { get; private set; }
        public int RoundNumber { get; private set; }
        public ICollection<ThemeAnswer> Answers { get; private set; }
    }
}
