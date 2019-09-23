using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WeStop.Api.Infra.Hubs
{
    public static class ConnectionBinding
    {
        private static readonly ConcurrentDictionary<string, (Guid PlayerId, Guid GameId)> _playersConnectionsIds = new ConcurrentDictionary<string, (Guid PlayerId, Guid GameId)>();
        private static readonly ConcurrentDictionary<Guid, ICollection<(string ConnectionId, Guid PlayerId)>> _gameConnectionsIds = new ConcurrentDictionary<Guid, ICollection<(string ConnectionId, Guid PlayerId)>>();

        public static void BindConnectionId(string connectionId, Guid playerId, Guid gameId)
        {
            _playersConnectionsIds.AddOrUpdate(connectionId, (playerId, gameId), (k, v) => v);
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

        public static bool RemoveConnectionIdBinding(string connectionId, out Guid? gameId, out Guid? playerId)
        {
            try
            {
                _playersConnectionsIds.TryRemove(connectionId, out (Guid PlayerId, Guid GameId) removedBinding);
                _gameConnectionsIds[removedBinding.GameId].Remove((connectionId, removedBinding.PlayerId));
                gameId = removedBinding.GameId;
                playerId = removedBinding.PlayerId;
                return true;
            }
            catch (Exception)
            {
                gameId = null;
                playerId = null;
                return false;
            }
        }

        public static ICollection<(string ConnectionId, Guid PlayerId)> GetGameConnections(Guid gameId) =>
            _gameConnectionsIds[gameId];
    }
}
