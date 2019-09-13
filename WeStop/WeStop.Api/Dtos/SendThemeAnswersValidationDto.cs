using System;
using System.Collections.Generic;
using WeStop.Api.Domain;

namespace WeStop.Api.Dtos
{
    public class SendThemeAnswersValidationDto
    {
        public Guid GameId { get; set; }
        public int RoundNumber { get; set; }
        public Guid UserId { get; set; }
        public ICollection<Validation> Validations { get; set; }
    }
}