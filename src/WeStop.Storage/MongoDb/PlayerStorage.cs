using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Core;
using WeStop.Core.Storages;

namespace WeStop.Storage.MongoDb
{
    public class PlayerStorage : IPlayerStorage
    {
        private readonly MongoContext _context;

        public PlayerStorage(MongoContext mongoContext)
        {
            _context = mongoContext;
        }

        public async Task AddAsync(Player player)
        {
            await _context.PlayersCollection.InsertOneAsync(player);
        }

        public async Task DeleteAsync(Guid gameId, Guid playerId)
        {
            var filter = Builders<Player>.Filter.Eq(x => x.GameId, gameId);
            filter &= Builders<Player>.Filter.Eq(x => x.Id, playerId);
            await _context.PlayersCollection.DeleteOneAsync(filter);
        }

        public async Task EditAsync(Player player)
        {
            var filter = Builders<Player>.Filter.Eq(x => x.Id, player.Id);
            await _context.PlayersCollection.ReplaceOneAsync(filter, player);
        }

        public async Task<ICollection<Player>> GetAllAsync(Guid gameId)
        {
            var filter = Builders<Player>.Filter.Eq(x => x.GameId, gameId);
            return await _context.PlayersCollection.Find(filter).ToListAsync();
        }

        public async Task<Player> GetAsync(Guid gameId, Guid playerId)
        {
            var filter = Builders<Player>.Filter.Eq(x => x.GameId, gameId);
            filter &= Builders<Player>.Filter.Eq(x => x.Id, playerId);
            return await _context.PlayersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Player>> GetPlayersInRoundAsync(Guid gameId)
        {
            var filter = Builders<Player>.Filter.Eq(x => x.GameId, gameId);
            filter &= Builders<Player>.Filter.Eq(x => x.InRound, true);
            return await _context.PlayersCollection.Find(filter).ToListAsync();
        }
    }
}