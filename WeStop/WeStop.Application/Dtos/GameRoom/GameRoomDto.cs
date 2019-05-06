using System;

namespace WeStop.Application.Dtos.GameRoom
{
    public class GameRoomDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int NumberOfRounds { get; set; }
        public int NumberOfPlayers { get; set; }
        public bool IsPrivate { get; set; }
    }
}
