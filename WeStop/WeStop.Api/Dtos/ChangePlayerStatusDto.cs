using System;

namespace WeStop.Api.Dtos
{
    public class ChangePlayerStatusDto
    {
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }
        public bool IsReady { get; set; }
    }
}