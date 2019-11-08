using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WeStop.Api.Infra.Hubs
{
    public class LobbyHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}