using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Classes;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IPlayerConnectionStorage
    {
        Task AddAsync(PlayerConnection playerConnection);
        Task UpdateAsync(PlayerConnection playerConnection);
        Task DeleteAsync(string connectionId);
        Task<bool> ExistsAsync(Guid playerId);
        Task<string[]> GetConnectionsIdsForGame(Guid gameId);
        Task<ICollection<PlayerConnection>> GetConnectionsForGameAsync(Guid gameId);
    }
}
