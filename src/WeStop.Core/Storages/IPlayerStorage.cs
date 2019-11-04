using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeStop.Core.Storages
{
    public interface IPlayerStorage
    {
        Task AddAsync(Player player);
        Task EditAsync(Player player);
        Task<Player> GetAsync(Guid gameId, Guid playerId);
        Task<ICollection<Player>> GetPlayersInRoundAsync(Guid gameId);
        Task<ICollection<Player>> GetAllAsync(Guid gameId);
        Task DeleteAsync(Guid gameId, Guid playerId);
    }
}
