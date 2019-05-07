using System;

namespace WeStop.Domain
{
    public class GameRoomPlayer
    {
        public GameRoomPlayer(Guid gameRoomId, Guid playerId, bool isAdmin)
        {
            GameRoomId = gameRoomId;
            PlayerId = playerId;
            IsAdmin = isAdmin;
        }

        public Guid GameRoomId { get; set; }
        public GameRoom GameRoom { get; set; }
        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
        public bool IsAdmin { get; set; }
    }
}
