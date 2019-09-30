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
            PlayerStorage = new PlayerStorage();
            AnswerStorage = new AnswerStorage();
            ValidationStorage = new ValidationStorage();
            PontuationStorage = new PontuationStorage();
        }

        public void CreateManagers()
        {
            GameManager = new GameManager(GameStorage, AnswerStorage,
                ValidationStorage, PontuationStorage, PlayerStorage);
        }

        public void CreateDefaultGame()
        {
            Game = GameManager.CreateAsync(TestUsers.Dustin, TestGame.Name, TestGame.Password, TestGame.Options).Result;
            GameManager.JoinAsync(Game.Id, TestUsers.Will, null).Wait();
            GameManager.JoinAsync(Game.Id, TestUsers.Lucas, null).Wait();
            GameManager.JoinAsync(Game.Id, TestUsers.Mike, null).Wait();
        }

        public Game CreateGame(User user, string name, string password, GameOptions options) =>
            Game = GameManager.CreateAsync(user, name, password, options).Result;

        public void CreateDefaultConfig()
        {
            CreateStorages();
            CreateManagers();
            CreateDefaultGame();
        }

        public IGameStorage GameStorage { get; set; }
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