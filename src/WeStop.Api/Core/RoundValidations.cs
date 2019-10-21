using System;
using System.Collections.Generic;

namespace WeStop.Api.Core
{
    public class RoundValidations
    {
        public RoundValidations(Guid gameId, int roundNumber, Guid playerId, string theme, ICollection<Validation> validations)
        {
            GameId = gameId;
            RoundNumber = roundNumber;
            PlayerId = playerId;
            Theme = theme;
            Validations = validations;
        }

        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public int RoundNumber { get; set; }
        public string Theme { get; set; }
        public ICollection<Validation> Validations { get; set; }
    }
}
