using System;
using System.Collections.Concurrent;

namespace WeStop.Api.Infra.Hubs
{
    public static class ConnectionBinding
    {
        private static ConcurrentDictionary<string, (Guid PlayerId, Guid GameId)> _connectionsIds = new ConcurrentDictionary<string, (Guid PlayerId, Guid GameId)>();

        public static void BindConnectionId(string connectionId, Guid playerId, Guid gameId) =>
            _connectionsIds.AddOrUpdate(connectionId, (playerId, gameId), (k, v) => v);

        public static void RemoveConnectionIdBinding(string connectionId) =>
            _connectionsIds.TryRemove(connectionId, out _);
    }
}
