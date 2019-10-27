using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Core;
using WeStop.Core.Storages;

namespace WeStop.Storage.MongoDb
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
            var playersFilter = Builders<Player>.Filter.Eq(x => x.GameId, id);
            await _context.PlayersCollection.DeleteManyAsync(playersFilter);
        }

        public async Task EditAsync(Game game)
        {
            var filter = Builders<Game>.Filter.Eq(x => x.Id, game.Id);
            await _context.GamesCollection.ReplaceOneAsync(filter,  game);
        }

        public async Task<ICollection<Game>> GetAsync()
        {
            var filter = Builders<Game>.Filter.Empty;
            return await _context.GamesCollection.Aggregate()
                .Match(filter)
                .Lookup<Game, Player, Game>(_context.PlayersCollection,
                    g => g.Id,
                    p => p.GameId,
                    g => g.Players).ToListAsync();
        }

        public async Task<Game> GetByIdAsync(Guid id)
        {
            var filter = Builders<Game>.Filter.Eq(x => x.Id, id);

            return await _context.GamesCollection.Aggregate()
                .Match(filter)
                .Lookup<Game, Player, Game>(_context.PlayersCollection,
                    g => g.Id,
                    p => p.GameId,
                    g => g.Players).FirstOrDefaultAsync();
        }

        public async Task<ICollection<string>> GetThemesAsync(Guid gameId)
        {
            var game = await GetByIdAsync(gameId);
            return game.Options.Themes;
        }
    }
}