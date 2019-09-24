using System;

namespace WeStop.Api.Dtos
{
    public class JoinToGameRoomDto
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
    }
}