using WeStop.Core;
using WeStop.Core.Services;
using WeStop.Core.Storages;
using WeStop.Storage.InMemory;
using WeStop.UnitTest.Helpers;

namespace WeStop.UnitTest
{
    public abstract class Test
    {    
        public IGameStorage GameStorage { get; set; }
        public IPlayerStorage PlayerStorage { get; set; }
        public GameManager GameManager { get; set; }
        public RoundScorer RoundScorer => new RoundScorer(GameStorage, PlayerStorage);
        public Game Game { get; private set; }

        public void CreateStorages()
        {
            GameStorage = new GameStorage();
            PlayerStorage = new PlayerStorage();
        }

        public void CreateManagers()
        {
            GameManager = new GameManager(GameStorage, PlayerStorage, RoundScorer);
        }

        public void CreateDefaultGame()
        {
            Game = GameManager.CreateAsync(TestUsers.Dustin, TestGame.Name, TestGame.Password, TestGame.Options).Result;
            GameManager.AuthorizePlayerAsync(Game.Id, string.Empty, TestUsers.Will).Wait();
            GameManager.AuthorizePlayerAsync(Game.Id, string.Empty, TestUsers.Lucas).Wait();
            GameManager.AuthorizePlayerAsync(Game.Id, string.Empty, TestUsers.Mike).Wait();
            GameManager.JoinAsync(Game.Id, TestUsers.Will, null, null).Wait();
            GameManager.JoinAsync(Game.Id, TestUsers.Lucas, null, null).Wait();
            GameManager.JoinAsync(Game.Id, TestUsers.Mike, null, null).Wait();
        }

        public Game CreateGame(User user, string name, string password, GameOptions options) =>
            Game = GameManager.CreateAsync(user, name, password, options).Result;

        public void CreateDefaultConfig()
        {
            CreateStorages();
            CreateManagers();
            CreateDefaultGame();
        }
    }
}