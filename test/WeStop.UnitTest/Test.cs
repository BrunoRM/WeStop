using System;
using WeStop.Api.Domain;
using WeStop.Api.Domain.Services;
using WeStop.Api.Infra.Storages.InMemory;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.Api.Managers;

namespace WeStop.UnitTest
{
    public abstract class Test
    {        
        public void CreateStorages()
        {
            GameStorage = new GameStorage();
            UserStorage = new UserStorage();
            PlayerStorage = new PlayerStorage();
            AnswerStorage = new AnswerStorage();
            ValidationStorage = new ValidationStorage();
            PontuationStorage = new PontuationStorage();
        }

        public void CreateManagers()
        {
            GameManager = new GameManager(GameStorage, UserStorage, AnswerStorage,
                ValidationStorage, PontuationStorage, PlayerStorage);
        }

        public void CreateDefaultGame()
        {
            UserStorage.CreateAsync(TestUsers.Dustin).Wait();
            UserStorage.CreateAsync(TestUsers.Will).Wait();
            UserStorage.CreateAsync(TestUsers.Lucas).Wait();
            UserStorage.CreateAsync(TestUsers.Mike).Wait();

            Game = GameManager.CreateAsync(TestUsers.Dustin.Id, TestGame.Name, TestGame.Password, TestGame.Options).Result;
            GameManager.JoinAsync(Game.Id, TestUsers.Will.Id, null).Wait();
            GameManager.JoinAsync(Game.Id, TestUsers.Lucas.Id, null).Wait();
            GameManager.JoinAsync(Game.Id, TestUsers.Mike.Id, null).Wait();
        }

        public Game CreateGame(Guid userId, string name, string password, GameOptions options) =>
            Game = GameManager.CreateAsync(userId, name, password, options).Result;

        public void CreateDefaultConfig()
        {
            CreateStorages();
            CreateManagers();
            CreateDefaultGame();
        }

        public IGameStorage GameStorage { get; set; }
        public IUserStorage UserStorage { get; set; }
        public IPlayerStorage PlayerStorage { get; set; }
        public IAnswerStorage AnswerStorage { get; set; }
        public IValidationStorage ValidationStorage { get; set; }
        public IPontuationStorage PontuationStorage { get; set; }
        public GameManager GameManager { get; set; }
        public RoundScorer RoundScorer => new RoundScorer(GameStorage, AnswerStorage, ValidationStorage,
            PontuationStorage);
        public Game Game { get; private set; }
    }
}