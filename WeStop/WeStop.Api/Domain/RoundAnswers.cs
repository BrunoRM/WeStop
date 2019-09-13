using System;
using System.Collections.Generic;

namespace WeStop.Api.Domain
{
    public class RoundAnswers
    {
        public RoundAnswers(Guid gameId, int roundNumber, ICollection<Answer> answers)
        {
            GameId = gameId;
            RoundNumber = roundNumber;
            Answers = answers;
        }

        public Guid PlayerId { get; private set; }
        public Guid GameId { get; private set; }
        public int RoundNumber { get; private set; }
        public ICollection<Answer> Answers { get; private set; }
    }
}
