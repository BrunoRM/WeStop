using MongoDB.Bson;
using MongoDB.Driver;

namespace WeStop.Api.Infra.Storages.MongoDb
{
    public class BaseStorage
    {
        protected const string THEMES_COLLECTION_NAME = "themes";
        protected const string GAMES_COLLECTION_NAME = "games";
        protected const string PLAYERS_COLLECTION_NAME = "players";
        protected const string SCOREBOARDS_COLLECTION_NAME = "scoreboards";

        protected readonly IMongoDatabase _database;

        public BaseStorage()
        {
            var client = new MongoClient();
            _database = client.GetDatabase("westop");
        }
    }
}