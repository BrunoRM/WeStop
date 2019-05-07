using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Application.Dtos.GameRoom;
using WeStop.Domain;
using WeStop.Infra;

namespace WeStop.Application.Queries.GetWaitingGameRooms
{
    public class GetGameRoomsQueryHandler : IRequestHandler<GetGameRoomsQuery, ICollection<GameRoomDto>>
    {
        private readonly WeStopDbContext _db;
        private readonly IMapper _mapper;

        public GetGameRoomsQueryHandler(WeStopDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ICollection<GameRoomDto>> Handle(GetGameRoomsQuery request, CancellationToken cancellationToken)
        {
            ICollection<GameRoom> gameRooms = new List<GameRoom>();

            if (request.OnlyPublic)
                gameRooms = await _db.GameRooms.Where(x => x.Password != string.Empty && x.Status == GameRoomStatus.Waiting).ToListAsync();
            else
                gameRooms = await _db.GameRooms.Where(x => x.Status == GameRoomStatus.Waiting).ToListAsync();

            return _mapper.Map<ICollection<GameRoom>, ICollection<GameRoomDto>>(gameRooms);
        }
    }
}
