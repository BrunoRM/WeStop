using MediatR;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Application.Dtos.Player;
using WeStop.Application.Errors;
using WeStop.Application.Exceptions;
using WeStop.Common.Handlers;
using WeStop.Domain;
using WeStop.Domain.Repositories;
using WeStop.Infra;
using WeStop.Infra.Extensions.Queries;

namespace WeStop.Application.Commands.RegisterPlayer
{
    public class RegisterPlayerCommandHandler : BaseRequestHandler<RegisterPlayerCommand>, IRequestHandler<RegisterPlayerCommand, PlayerDto>
    {
        private readonly WeStopDbContext _db;

        public RegisterPlayerCommandHandler(IPlayerRepository playerRepository, WeStopDbContext db)
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

            string passwordHash = GetMD5Hash(request.Password);
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

        private string GetMD5Hash(string input)
        {
            using (var md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
    }
}
