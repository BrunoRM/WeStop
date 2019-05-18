using System;
using WeStop.Api.Classes;

namespace WeStop.Api.Dtos
{
    public class SendThemeAnswersValidationDto
    {
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }
        public ThemeValidation Validation { get; set; }
    }
}