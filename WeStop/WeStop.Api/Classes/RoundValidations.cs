using System;
using System.Collections.Generic;

namespace WeStop.Api.Classes
{
    public class RoundValidations
    {
        public RoundValidations(Guid roundId, ICollection<ThemeValidation> validations)
        {
            RoundId = roundId;
            Validations = validations;
        }

        public Guid RoundId { get; set; }
        public ICollection<ThemeValidation> Validations { get; set; }
    }
}
