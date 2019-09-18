using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages.InMemory
{
    public class PlayerStorage : IPlayerStorage
    {
        private static readonly ICollection<Player> _players = new List<Player>();

        public Task AddAsync(Player player)
        {
            _players.Add(player);
            return Task.CompletedTask;
        }

        public Task<Player> GetAsync(Guid gameId, Guid playerId)
        {
            return Task.FromResult(_players.FirstOrDefault(p => p.Id == playerId));
        }

        public Task<ICollection<Player>> GetPlayersAsync(Guid gameId)
        {
            return Task.FromResult<ICollection<Player>>(
                _players.Where(p => p.GameId == gameId).ToList());
        }

        public Task UpdateAsync(Player player)
        {
            return Task.CompletedTask;
        }
    }
}
