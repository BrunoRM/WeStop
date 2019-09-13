using System;
using System.Collections.Generic;
using WeStop.Api.Domain;

namespace WeStop.Api.Dtos
{
    public class SendAnswersDto
    {
        public Guid GameId { get; set; }
        public int RoundNumber { get; set; }
        public Guid UserId { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}