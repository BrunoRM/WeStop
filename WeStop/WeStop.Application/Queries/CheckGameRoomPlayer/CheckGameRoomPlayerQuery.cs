using MediatR;
using System;

namespace WeStop.Application.Queries.CheckGameRoomPlayer
{
    public class CheckGameRoomPlayerQuery : IRequest<bool>
    {
        public Guid GameRoomId { get; set; }
        public string UserName { get; set; }
    }
}
