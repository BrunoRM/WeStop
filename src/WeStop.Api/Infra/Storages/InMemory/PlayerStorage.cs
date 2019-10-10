using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Core;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages.InMemory
{
    public class PlayerStorage : IPlayerStorage
    {
        private readonly ICollection<Player> _players = new List<Player>();

        public Task AddAsync(Player player)
        {
            _players.Add(player);
            return Task.CompletedTask;
        }

        public void Edit(Player player)
        {
            return;
        }

        public Task EditAsync(Player player) =>
            Task.Run(() => Edit(player));

        public Player Get(Guid gameId, Guid playerId) =>
            _players.FirstOrDefault(p => p.Id == playerId);

        public Task<Player> GetAsync(Guid gameId, Guid playerId)
        {
            return Task.FromResult(Get(gameId, playerId));
        }

        public ICollection<Player> GetPlayersInRound(Guid gameId) =>
            _players.Where(p => p.InRound && p.GameId == gameId).ToList();

        public Task<ICollection<Player>> GetPlayersInRoundAsync(Guid gameId)
        {
            return Task.FromResult<ICollection<Player>>(GetPlayersInRound(gameId));
        }  

        public ICollection<Player> GetAll(Guid gameId) =>
            _players.Where(p => p.GameId == gameId).ToList();
    }
}
