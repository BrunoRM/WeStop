using System;
using System.Collections.Generic;

namespace WeStop.Api.Domain
{
    public class RoundValidations
    {
        public RoundValidations(Guid gameId, int roundNumber, Guid playerId, ICollection<Validation> validations)
        {
            GameId = gameId;
            RoundNumber = roundNumber;
            PlayerId = playerId;
            Validations = validations;
        }

        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public int RoundNumber { get; set; }
        public ICollection<Validation> Validations { get; set; }
    }
}
