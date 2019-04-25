using System.Threading.Tasks;
using WeStop.Application.Dtos.Player;
using WeStop.Common.AppServices;

namespace WeStop.Application.Services.Interfaces
{
    public interface IPlayerAppService
    {
        Task<Result<RegisterPlayerDto>> RegisterPlayer(RegisterPlayerDto dto);
    }
}
