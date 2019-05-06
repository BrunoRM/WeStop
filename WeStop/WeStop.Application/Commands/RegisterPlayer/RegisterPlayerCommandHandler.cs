using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Application.Dtos.Player;
using WeStop.Application.Exceptions;
using WeStop.Domain;
using WeStop.Domain.Errors;
using WeStop.Helpers.Criptography;
using WeStop.Infra;
using WeStop.Infra.Extensions.Queries;

namespace WeStop.Application.Commands.RegisterPlayer
{
    public class RegisterPlayerCommandHandler : IRequestHandler<RegisterPlayerCommand, PlayerDto>
    {
        private readonly WeStopDbContext _db;

        public RegisterPlayerCommandHandler(WeStopDbContext db)
        {
            _db = db;
        }

        public async Task<PlayerDto> Handle(RegisterPlayerCommand request, CancellationToken cancellationToken)
        {
            request.Name = request.Name.Trim();
            request.UserName = request.UserName.Trim();
            request.Email = request.Email.Trim();

            if (await _db.Players.UserNameExistsAsync(request.UserName))
                throw new ErrorException(PlayerErrors.UserNameAlreadyTaken);

            if (await _db.Players.EmailExistsAsync(request.Email))
                throw new ErrorException(PlayerErrors.EmailAlreadyTaken);

            string passwordHash = new MD5HashGenerator().GetMD5Hash(request.Password);
            var player = new Player(request.Name, request.UserName, passwordHash, request.Email);

            await _db.Players.AddAsync(player);
            await _db.SaveChangesAsync();

            return new PlayerDto
            {
                Id = player.Id,
                Name = request.Name,
                Email = request.Email,
                UserName = request.UserName
            };
        }
    }
}
