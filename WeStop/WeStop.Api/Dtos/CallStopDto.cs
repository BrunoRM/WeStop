using System;

namespace WeStop.Api.Dtos
{
    public class CallStopDto
    {
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }
    }
}