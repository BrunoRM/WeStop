using System;
using System.Collections.Generic;

namespace WeStop.Api.Classes
{
    public class RoundValidations
    {
        public RoundValidations(Guid gameId, int roundNumber, ICollection<ThemeValidation> validations)
        {
            GameId = gameId;
            RoundNumber = roundNumber;
            Validations = validations;
        }

        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public int RoundNumber { get; set; }
        public ICollection<ThemeValidation> Validations { get; set; }
    }
}
