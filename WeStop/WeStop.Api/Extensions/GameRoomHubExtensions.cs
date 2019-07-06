using Microsoft.AspNetCore.SignalR;
using System;
using WeStop.Api.Infra.Hubs;

namespace WeStop.Api.Extensions
{
    public static class GameRoomHubExtensions
    {
        public static IClientProxy GetGameGroup(this IHubContext<GameRoomHub> hub, Guid gameId) =>
            hub.Clients.Group(gameId.ToString());
    }
}
