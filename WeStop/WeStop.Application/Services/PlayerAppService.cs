using System.Threading.Tasks;
using WeStop.Application.Dtos.Player;
using WeStop.Application.Services.Interfaces;
using WeStop.Application.Validators.Player;
using WeStop.Common.AppServices;
using WeStop.Domain.Repositories;

namespace WeStop.Application.Services
{
    public class PlayerAppService : ApplicationServiceBase, IPlayerAppService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerAppService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<Result<RegisterPlayerDto>> RegisterPlayer(RegisterPlayerDto dto)
        {
            if (!await IsValidAsync<RegisterPlayerDto, RegisterPlayerDtoValidator>(dto))
                return await RequestValidationErrors<RegisterPlayerDto>();

            dto.Name = dto.Name.Trim();
            dto.UserName = dto.UserName.Trim();

            if (await _playerRepository.UserNameAlreadyExistsAsync(dto.UserName))
                return await Error<RegisterPlayerDto>(ERRORS.USERNAME_ALREADY_EXISTS);

            if (await _playerRepository.EmailAlreadyExistsAsync(dto.Email))
                return await Error<RegisterPlayerDto>(ERRORS.EMAIL_ALREADY_TAKEN);

            return await Ok("Usuário cadastrado com sucesso", dto);
        }
    }
}
