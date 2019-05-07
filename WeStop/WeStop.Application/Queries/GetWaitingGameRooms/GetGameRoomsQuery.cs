using MediatR;
using System.Collections.Generic;
using WeStop.Application.Dtos.GameRoom;

namespace WeStop.Application.Queries.GetWaitingGameRooms
{
    public class GetGameRoomsQuery : IRequest<ICollection<GameRoomDto>>
    {
        public bool OnlyPublic { get; set; }
    }
}
