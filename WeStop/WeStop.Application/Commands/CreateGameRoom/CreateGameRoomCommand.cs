using MediatR;
using WeStop.Application.Dtos.GameRoom;
using WeStop.Common.Handlers;

namespace WeStop.Application.Commands.RegisterPlayer
{
    public class CreateGameRoomCommand : IRequest<Response<GameRoomDto>>
    {
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
    }
}
