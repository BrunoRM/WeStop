using System;

namespace WeStop.Api.Classes
{
    public class PlayerConnection
    {
        public PlayerConnection(string connectionId, Guid playerId, Guid gameId)
        {
            ConnectionId = connectionId;
            PlayerId = playerId;
            GameId = gameId;
        }

        public string ConnectionId { get; private set; }
        public Guid PlayerId { get; private set; }
        public Guid GameId { get; private set; }

        public void UpdateConnectionId(string connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}
