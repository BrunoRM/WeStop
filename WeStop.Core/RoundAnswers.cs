using System;
using System.Collections.Generic;

namespace WeStop.Core
{
    public class RoundAnswers
    {
        public RoundAnswers(Guid gameId, int roundNumber, Guid playerId, ICollection<Answer> answers)
        {
            GameId = gameId;
            RoundNumber = roundNumber;
            PlayerId = playerId;
            Answers = answers;
        }

        public Guid PlayerId { get; private set; }
        public Guid GameId { get; private set; }
        public int RoundNumber { get; private set; }
        public ICollection<Answer> Answers { get; private set; }
    }
}
