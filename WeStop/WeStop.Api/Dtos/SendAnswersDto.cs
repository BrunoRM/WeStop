using System;
using System.Collections.Generic;
using WeStop.Api.Classes;

namespace WeStop.Api.Dtos
{
    public class SendAnswersDto
    {
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }
        public ICollection<ThemeAnswer> Answers { get; set; }
    }
}