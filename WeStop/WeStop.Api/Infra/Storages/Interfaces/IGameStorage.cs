using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Classes;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IGameStorage
    {
        Task CreateAsync(Game game);
        Task<Game> GetByIdAsync(Guid id);
        Task<ICollection<Game>> GetAsync();
    }
}