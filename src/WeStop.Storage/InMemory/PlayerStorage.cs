using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Core;
using WeStop.Core.Storages;

namespace WeStop.Storage.InMemory
{
    public class PlayerStorage : IPlayerStorage
    {
        private readonly ICollection<Player> _players = new List<Player>();

        public Task AddAsync(Player player)
        {
            _players.Add(player);
            return Task.CompletedTask;
        }

        public Task EditAsync(Player player) =>
            Task.CompletedTask;

        public Task<Player> GetAsync(Guid gameId, Guid playerId)
        {
            return Task.FromResult(_players.FirstOrDefault(p => p.GameId == gameId && p.Id == playerId));
        }

        public Task<ICollection<Player>> GetPlayersInRoundAsync(Guid gameId)
        {
            return Task.FromResult<ICollection<Player>>(_players.Where(p => p.GameId == gameId && p.InRound).ToList());
        }  

        public Task<ICollection<Player>> GetAllAsync(Guid gameId) =>
            Task.FromResult<ICollection<Player>>(_players.Where(p => p.GameId == gameId).ToList());
    }
}
