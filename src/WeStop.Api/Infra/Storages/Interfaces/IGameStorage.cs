using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Core;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IGameStorage
    {
        Task AddAsync(Game game);
        void Edit(Game game);
        Task EditAsync(Game game);
        Game GetById(Guid id);
        Task<Game> GetByIdAsync(Guid id);
        Task<ICollection<Game>> GetAsync();
        ICollection<string> GetThemes(Guid gameId);
        Task<ICollection<string>> GetThemesAsync(Guid gameId);
    }
}