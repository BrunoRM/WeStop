using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Application.Dtos.GameRoom;
using WeStop.Application.Exceptions;
using WeStop.Domain;
using WeStop.Domain.Errors;
using WeStop.Helpers.Criptography;
using WeStop.Infra;
using WeStop.Infra.Extensions.Queries;

namespace WeStop.Application.Commands.RegisterPlayer
{
    public class CreateGameRoomCommandHandler : IRequestHandler<CreateGameRoomCommand, GameRoomDto>
    {
        private readonly WeStopDbContext _db;

        public CreateGameRoomCommandHandler(WeStopDbContext db)
        {
            _db = db;
        }

        public async Task<GameRoomDto> Handle(CreateGameRoomCommand request, CancellationToken cancellationToken)
        {
            request.Name = request.Name.Trim();

            if (await _db.GameRooms.NameExistsAsync(request.Name))
                throw new ErrorException(GameRoomErrors.NameAlreadyExists);

            string passwordHash = string.Empty;
            if (!string.IsNullOrEmpty(request.Password))
                passwordHash = new MD5HashGenerator().GetMD5Hash(request.Password);

            GameRoom gameRoom = new GameRoom(request.Name, passwordHash, request.NumberOfRounds, request.NumberOfPlayers);

            await _db.GameRooms.AddAsync(gameRoom);
            await _db.SaveChangesAsync();

            return new GameRoomDto
            {
                Id = gameRoom.Id,
                Name = gameRoom.Name,
                Status = gameRoom.Status.ToString("g"),
                NumberOfRounds = gameRoom.NumberOfRounds,
                NumberOfPlayers = gameRoom.NumberOfPlayers,
                IsPrivate = string.IsNullOrEmpty(gameRoom.Password) ? false : true
            };
        }
    }
}
