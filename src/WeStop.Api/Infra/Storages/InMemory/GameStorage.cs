using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Core;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages.InMemory
{
    public class GameStorage : IGameStorage
    {
        private readonly ICollection<Game> _games = new List<Game>();
        
        public Task AddAsync(Game game) =>
            Task.Run(() => _games.Add(game));

        public Task EditAsync(Game game) =>
            Task.CompletedTask;

        public Task<Game> GetByIdAsync(Guid id) =>
            Task.FromResult(_games.FirstOrDefault(g => g.Id == id));

        public Task<ICollection<Game>> GetAsync() =>
            Task.FromResult<ICollection<Game>>(_games.ToList());

        public async Task<ICollection<string>> GetThemesAsync(Guid gameId)
        {
            var game = await GetByIdAsync(gameId);
            return game.Options.Themes;
        }

        public async Task<Game> GetByIdWithPlayersAsync(Guid id) =>
            await GetByIdAsync(id);

        public async Task<ICollection<Game>> GetWithPlayersAsync() =>
            await GetAsync();

        public Task DeleteAsync(Guid id)
        {
            _games.Remove(_games.First(x => x.Id == id));
            return Task.CompletedTask;
        }
    }
}