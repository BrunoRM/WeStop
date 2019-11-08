using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Infra.Hubs
{
    public static class ConnectionBinding
    {
        private static readonly ConcurrentDictionary<string, (Guid PlayerId, Guid GameId)> _playersConnectionsIds = new ConcurrentDictionary<string, (Guid PlayerId, Guid GameId)>();
        private static readonly ConcurrentDictionary<Guid, ConcurrentBag<(string ConnectionId, Guid PlayerId)>> _gameConnectionsIds = new ConcurrentDictionary<Guid, ConcurrentBag<(string ConnectionId, Guid PlayerId)>>();

        public static void BindConnectionId(string connectionId, Guid playerId, Guid gameId)
        {
            _playersConnectionsIds.AddOrUpdate(connectionId, (playerId, gameId), (k, v) => v);
            if (!_gameConnectionsIds.ContainsKey(gameId))
            {
                var connections = new ConcurrentBag<(string ConnectionId, Guid PlayerId)>
                {
                    (connectionId, playerId)
                };

                _gameConnectionsIds.AddOrUpdate(gameId, connections, (k, v) => v);
            }
            else
                _gameConnectionsIds[gameId].Add((connectionId, playerId));
        }

        public static (Guid? playerId, Guid? gameId) RemoveConnectionIdBinding(string connectionId)
        {
            if (_playersConnectionsIds.ContainsKey(connectionId))
            {
                _playersConnectionsIds.TryRemove(connectionId, out (Guid PlayerId, Guid GameId) removedBinding);
                _ = _gameConnectionsIds[removedBinding.GameId].TryTake(out _);

                if (!_gameConnectionsIds[removedBinding.GameId].Any())
                {
                    _ = _gameConnectionsIds.TryRemove(removedBinding.GameId, out _);
                }

                return (removedBinding.PlayerId, removedBinding.GameId);
            }

            return (null, null);
        }

        public static ICollection<(string ConnectionId, Guid PlayerId)> GetGameConnections(Guid gameId) =>
            _gameConnectionsIds[gameId].ToList();

        public static string GetPlayerConnectionId(Guid gameId, Guid playerId) =>
            GetGameConnections(gameId).FirstOrDefault(x => x.PlayerId == playerId).ConnectionId;
    }
}
