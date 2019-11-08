using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeStop.Api.Extensions
{
    public static class GameHubExtensions
    {
        private static string GetGroupName(Guid gameId, int roundNumber)
        {
            return $"{gameId}_{roundNumber}";
        }

        public static async Task AddConnectionToGameRoundGroupAsync(this IGroupManager groups, Guid gameId, int roundNumber, string connectionId)
        {
            string gameRoundGroupName = GetGroupName(gameId, roundNumber);
            await groups.AddToGroupAsync(connectionId, gameRoundGroupName);
        }

        public static async Task AddConnectionsToGameRoundGroupAsync(this IGroupManager groups, Guid gameId, int roundNumber, ICollection<string> connectionsIds)
        {
            string gameRoundGroupName = GetGroupName(gameId, roundNumber);
            foreach (var connectionId in connectionsIds)
            {
                await groups.AddToGroupAsync(connectionId, gameRoundGroupName);
            }
        }

        public static IClientProxy GameRoundGroup(this IHubCallerClients clients, Guid gameId, int roundNumber)
        {
            var gameRoundGroupName = GetGroupName(gameId, roundNumber);
            return clients.Group(gameRoundGroupName);
        }

        public static IClientProxy GameRoundGroup(this IHubClients clients, Guid gameId, int roundNumber)
        {
            var gameRoundGroupName = GetGroupName(gameId, roundNumber);
            return clients.Group(gameRoundGroupName);
        }
    }
}
