using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Core;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IPlayerStorage
    {
        Task AddAsync(Player player);
        void Edit(Player player);
        Task EditAsync(Player player);
        Player Get(Guid gameId, Guid playerId);
        Task<Player> GetAsync(Guid gameId, Guid playerId);
        ICollection<Player> GetPlayersInRound(Guid gameId);
        Task<ICollection<Player>> GetPlayersInRoundAsync(Guid gameId);
        ICollection<Player> GetAll(Guid gameId);
    }
}
