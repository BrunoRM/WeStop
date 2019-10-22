using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Core;

namespace WeStop.Api.Infra.Storages.Interfaces
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