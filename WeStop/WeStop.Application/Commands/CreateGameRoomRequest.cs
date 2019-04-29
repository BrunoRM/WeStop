using MediatR;
using WeStop.Application.Dtos.GameRoom;
using WeStop.Common.Handlers;

namespace WeStop.Application.Commands
{
    public class CreateGameRoomRequest : IRequest<Response<GameRoomDto>>
    {
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
    }
}
