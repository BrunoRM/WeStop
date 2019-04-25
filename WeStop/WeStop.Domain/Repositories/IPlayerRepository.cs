using System.Threading.Tasks;

namespace WeStop.Domain.Repositories
{
    public interface IPlayerRepository
    {
        Task AddAsync(Player player);
        Task<bool> UserNameAlreadyExistsAsync(string userName);
        Task<bool> EmailAlreadyExistsAsync(string email);
    }
}
