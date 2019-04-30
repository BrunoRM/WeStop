using FluentValidation;
using MediatR;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeStop.Application.Commands;
using WeStop.Application.Dtos.Player;
using WeStop.Application.Errors;
using WeStop.Application.Exceptions;
using WeStop.Common.Handlers;
using WeStop.Domain;
using WeStop.Domain.Repositories;

namespace WeStop.Application.Handlers
{
    public class RegisterPlayerRequestHandler : BaseRequestHandler<RegisterPlayerRequest>, IRequestHandler<RegisterPlayerRequest, PlayerDto>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IValidator<RegisterPlayerRequest> _validator;

        public RegisterPlayerRequestHandler(IValidator<RegisterPlayerRequest> validator)
        {
            _validator = validator;
        }

        public RegisterPlayerRequestHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<PlayerDto> Handle(RegisterPlayerRequest request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request);

            request.Name = request.Name.Trim();
            request.UserName = request.UserName.Trim();
            request.Email = request.Email.Trim();

            if (await _playerRepository.UserNameAlreadyExistsAsync(request.UserName))
                throw new ErrorException(PlayerErrors.UserNameAlreadyTaken);

            if (await _playerRepository.EmailAlreadyExistsAsync(request.Email))
                throw new ErrorException(PlayerErrors.EmailAlreadyTaken);

            string passwordHash = GetMD5Hash(request.Password);
            var player = new Player(request.Name, request.UserName, passwordHash, request.Email);
            await _playerRepository.AddAsync(player);

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
