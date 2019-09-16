using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WeStop.Api.Infra.Hubs
{
    public static class ConnectionBinding
    {
        private static ConcurrentDictionary<string, (Guid PlayerId, Guid GameId)> _connectionsIds = new ConcurrentDictionary<string, (Guid PlayerId, Guid GameId)>();
        private static ConcurrentDictionary<Guid, ICollection<(string ConnectionId, Guid PlayerId)>> _gameConnectionsIds = new ConcurrentDictionary<Guid, ICollection<(string ConnectionId, Guid PlayerId)>>();

        public static void BindConnectionId(string connectionId, Guid playerId, Guid gameId)
        {
            _connectionsIds.AddOrUpdate(connectionId, (playerId, gameId), (k, v) => v);
            if (!_gameConnectionsIds.ContainsKey(gameId))
            {
                var connections = new List<(string ConnectionId, Guid PlayerId)>
                {
                    (connectionId, playerId)
                };

                _gameConnectionsIds.AddOrUpdate(gameId, connections, (k, v) => v);
            }
            else
                _gameConnectionsIds[gameId].Add((connectionId, playerId));
        }

        public static void RemoveConnectionIdBinding(string connectionId)
        {
            _connectionsIds.TryRemove(connectionId, out (Guid PlayerId, Guid GameId) removedBinding);
            _gameConnectionsIds[removedBinding.GameId].Remove((connectionId, removedBinding.PlayerId));
        }

        public static ICollection<(string ConnectionId, Guid PlayerId)> GetGameConnections(Guid gameId) =>
            _gameConnectionsIds[gameId];
    }
}
