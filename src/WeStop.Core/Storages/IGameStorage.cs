using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeStop.Core.Storages
{
    public interface IGameStorage
    {
        Task AddAsync(Game game);
        Task EditAsync(Game game);
        Task<Game> GetByIdAsync(Guid id);
        Task<ICollection<Game>> GetAsync();
        Task<ICollection<string>> GetThemesAsync(Guid gameId);
        Task DeleteAsync(Guid id);
    }
}