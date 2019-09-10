using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Classes;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages.InMemory
{
    public class PlayerConnectionStorage : IPlayerConnectionStorage
    {
        private static ICollection<PlayerConnection> _playersConnections = new List<PlayerConnection>();

        public Task AddAsync(PlayerConnection playerConnection)
        {
            _playersConnections.Add(playerConnection);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string connectionId)
        {
            PlayerConnection playerConnection = _playersConnections.FirstOrDefault(pc => pc.ConnectionId.Equals(connectionId));

            if (playerConnection != null)
            {
                _playersConnections.Remove(playerConnection);
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(Guid playerId)
        {
            bool exists = _playersConnections.Any(pc => pc.PlayerId == playerId);

            return Task.FromResult(exists);
        }

        public Task<ICollection<PlayerConnection>> GetConnectionsForGameAsync(Guid gameId)
        {
            var connections = _playersConnections.Where(pc => pc.GameId == gameId).ToList();
            return Task.FromResult<ICollection<PlayerConnection>>(connections);
        }

        public Task<string[]> GetConnectionsIdsForGame(Guid gameId)
        {
            string[] connectionsIds = _playersConnections.Where(pc => pc.GameId == gameId).Select(pc => pc.ConnectionId).ToArray();

            return Task.FromResult(connectionsIds);
        }

        public Task UpdateAsync(PlayerConnection playerConnection)
        {
            Guid gameId = playerConnection.GameId;
            Guid playerId = playerConnection.PlayerId;

            PlayerConnection existingPlayerConnection = _playersConnections.FirstOrDefault(pc => pc.GameId == gameId && pc.PlayerId == playerId);

            string connectionId = playerConnection.ConnectionId;
            existingPlayerConnection.UpdateConnectionId(connectionId);

            return Task.CompletedTask;
        }
    }
}
