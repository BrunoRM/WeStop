using System;
using System.Collections.Generic;
using WeStop.Api.Classes;

namespace WeStop.Api.Dtos
{
    public class SendThemeAnswersValidationDto
    {
        public Guid GameId { get; set; }
        public int RoundNumber { get; set; }
        public Guid UserId { get; set; }
        public ICollection<ThemeValidation> Validations { get; set; }
    }
}