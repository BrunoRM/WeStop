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

        public void Edit(Game game)
        {
            return;
        }

        public Task EditAsync(Game game) =>
            Task.Run(() => Edit(game));

        public Game GetById(Guid id) =>
            _games.FirstOrDefault(g => g.Id == id);

        public Task<Game> GetByIdAsync(Guid id) =>
            Task.FromResult(GetById(id));

        public Task<ICollection<Game>> GetAsync() =>
            Task.FromResult<ICollection<Game>>(_games.ToList());

        public ICollection<string> GetThemes(Guid gameId)
        {
            var game = GetById(gameId);
            return game.Options.Themes;
        }
    }
}