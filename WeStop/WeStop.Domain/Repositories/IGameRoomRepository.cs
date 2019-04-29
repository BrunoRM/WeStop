using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeStop.Domain.Repositories
{
    public interface IGameRoomRepository
    {
        Task<ICollection<GameRoom>> GetAsync();
        Task AddAsync(GameRoom game);
        Task<bool> NameAlreadyExistsAsync(string name);
    }
}
