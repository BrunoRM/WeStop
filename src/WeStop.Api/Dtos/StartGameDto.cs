using System;

namespace WeStop.Api.Dtos
{
    public class StartGameDto
    {
        public Guid GameRoomId { get; set; }
        public Guid UserId { get; set; }
    }
}