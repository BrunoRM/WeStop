using System;

namespace WeStop.Api.Dtos
{
    public class ConnectToGameRoomDto
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
    }
}