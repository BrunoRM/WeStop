using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WeStop.Api.Core;

namespace WeStop.Api.Infra.Storages.MongoDb
{
    public class MongoContext
    {
        private IMongoDatabase _database;

        public MongoContext(string connectionString)
        {
            BsonClassMap.RegisterClassMap<Game>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(x => x.Id)
                    .SetElementName("id");

                cm.MapMember(x => x.Name)
                    .SetElementName("name");

                cm.UnmapMember(x => x.Players);
            });
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("westop");
        }

        public IMongoCollection<Game> GamesCollection => _database.GetCollection<Game>(Consts.GAMES_COLLECTION_NAME);
        public IMongoCollection<Player> PlayersCollection => _database.GetCollection<Player>(Consts.PLAYERS_COLLECTION_NAME);
    }
}