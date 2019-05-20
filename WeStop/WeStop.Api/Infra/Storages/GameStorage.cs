using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Classes;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages
{
    public class GameStorage : IGameStorage
    {
        private readonly ICollection<Game> _games = new List<Game>();
        
        public Task CreateAsync(Game game) =>
            Task.Run(() => _games.Add(game));

        public Task<ICollection<Game>> GetAsync() =>
            Task.FromResult<ICollection<Game>>(_games.ToList());

        public Task<Game> GetByIdAsync(Guid id) =>
            Task.FromResult(_games.FirstOrDefault(g => g.Id == id));
    }
}