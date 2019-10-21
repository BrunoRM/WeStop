using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Core;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages.MongoDb
{
    public class GameStorage : BaseStorage, IGameStorage
    {
        public async Task AddAsync(Game game)
        {
            await _database.GetCollection<Game>(GAMES_COLLECTION_NAME).InsertOneAsync(game);
        }

        public void Edit(Game game)
        {
            //_database.GetCollection<Game>(GAMES_COLLECTION_NAME).UpdateOne(game);
        }

        public Task EditAsync(Game game)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Game>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Game GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Game> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetThemes(Guid gameId)
        {
            throw new NotImplementedException();
        }
    }
}