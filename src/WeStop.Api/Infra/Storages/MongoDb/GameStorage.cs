using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using WeStop.Api.Core;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages.MongoDb
{
    public class GameStorage : IGameStorage
    {
        private readonly MongoContext _context;

        public GameStorage(MongoContext mongoContext)
        {
            _context = mongoContext;
        }

        public async Task AddAsync(Game game)
        {
            await _context.GamesCollection.InsertOneAsync(game);
        }

        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<Game>.Filter.Eq(x => x.Id, id);
            await _context.GamesCollection.DeleteOneAsync(filter);
        }

        public async Task EditAsync(Game game)
        {
            var filter = Builders<Game>.Filter.Eq(x => x.Id, game.Id);
            await _context.GamesCollection.ReplaceOneAsync(filter,  game);
        }

        public async Task<ICollection<Game>> GetAsync()
        {
            var filter = Builders<Game>.Filter.Empty;
            return await _context.GamesCollection.Find(filter).ToListAsync();
        }

        public async Task<Game> GetByIdAsync(Guid id)
        {
            var filter = Builders<Game>.Filter.Eq(x => x.Id, id);
            return await _context.GamesCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<ICollection<string>> GetThemesAsync(Guid gameId)
        {
            var game = await GetByIdAsync(gameId);
            return game.Options.Themes;
        }
    }
}