using Microsoft.AspNetCore.SignalR;
using System;
using WeStop.Api.Infra.Hubs;

namespace WeStop.Api.Extensions
{
    public static class GameRoomHubExtensions
    {
        public static IClientProxy Group(this IHubContext<GameRoomHub> hub, Guid id) =>
            hub.Clients.Group(id.ToString());

        public static IClientProxy Group(this IHubCallerClients hub, Guid id) =>
            hub.Group(id.ToString());
    }
}
