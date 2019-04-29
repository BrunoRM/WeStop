using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Application.Commands;
using WeStop.Application.Dtos.GameRoom;
using WeStop.Application.Errors;
using WeStop.Common.Handlers;
using WeStop.Domain;
using WeStop.Domain.Repositories;

namespace WeStop.Application.Handlers
{
    public class CreateGameRoomRequestHandler : BaseRequestHandler<CreateGameRoomRequest>, IRequestHandler<CreateGameRoomRequest, Response<GameRoomDto>>
    {
        private readonly IGameRoomRepository _gameRoomRepository;

        public CreateGameRoomRequestHandler(IGameRoomRepository gameRoomRepository)
        {
            _gameRoomRepository = gameRoomRepository;
        }

        public async Task<Response<GameRoomDto>> Handle(CreateGameRoomRequest request, CancellationToken cancellationToken)
        {
            request.Name = request.Name.Trim();

            if (await _gameRoomRepository.NameAlreadyExistsAsync(request.Name))
                return new Response<GameRoomDto>(GameRoomErrors.NameAlreadyExists);

            GameRoom gameRoom = new GameRoom(request.Name, request.IsPrivate);

            await _gameRoomRepository.AddAsync(gameRoom);

            return new Response<GameRoomDto>(new GameRoomDto
            {
                Id = gameRoom.Id,
                Name = gameRoom.Name,
                Status = gameRoom.Status
            });
        }
    }
}
