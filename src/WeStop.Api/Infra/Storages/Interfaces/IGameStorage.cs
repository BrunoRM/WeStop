using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Domain;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IGameStorage
    {
        Task AddAsync(Game game);
        Task EditAsync(Game game);
        Task<Game> GetByIdAsync(Guid id);
        Task<ICollection<Game>> GetAsync();
        Task<ICollection<string>> GetThemesAsync(Guid gameId);
    }
}