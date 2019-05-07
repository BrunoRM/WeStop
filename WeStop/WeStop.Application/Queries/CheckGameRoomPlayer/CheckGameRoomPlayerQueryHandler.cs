using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Application.Exceptions;
using WeStop.Domain.Errors;
using WeStop.Infra;

namespace WeStop.Application.Queries.CheckGameRoomPlayer
{
    public class CheckGameRoomPlayerQueryHandler : IRequestHandler<CheckGameRoomPlayerQuery, bool>
    {
        private readonly WeStopDbContext _db;

        public CheckGameRoomPlayerQueryHandler(WeStopDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Handle(CheckGameRoomPlayerQuery request, CancellationToken cancellationToken)
        {
            var gameRoom = await _db.GameRooms.Include(x => x.Players)
                .FirstOrDefaultAsync(x => x.Id == request.GameRoomId);

            if (gameRoom is null)
                throw new NotFoundException(GameRoomErrors.GameRoomNotFound);

            var player = await _db.Players.FirstOrDefaultAsync(x => x.UserName == request.UserName);

            if (player is null)
                throw new NotFoundException(PlayerErrors.PlayerNotFound);

            return gameRoom.Players.Any(x => x.PlayerId == player.Id);
        }
    }
}
