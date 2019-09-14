using NUnit.Framework;
using WeStop.Api.Domain;
using WeStop.Api.Helpers;
using WeStop.Api.Infra.Storages.InMemory;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.UnitTest.Extensions;
using WeStop.UnitTest.Helpers;

namespace WeStop.UnitTest
{
    [TestFixture]
    public class RoundScorerTests
    {
        private readonly IGameStorage _gameStorage;
        private readonly IAnswerStorage _answerStorage;
        private readonly IValidationStorage _validationStorage;
        private readonly IPontuationStorage _gamePontuationStorage;
        private readonly RoundScorer _roundScorer;
        private Game _game;

        public RoundScorerTests()
        {
            _gameStorage = new GameStorage();
            _answerStorage = new AnswerStorage();
            _validationStorage = new ValidationStorage();
            _gamePontuationStorage = new PontuationStorage();
            _roundScorer = new RoundScorer(_gameStorage, _answerStorage, _validationStorage, _gamePontuationStorage);
            Setup();
        }

        public void Setup()
        {
            _game = TestGame.Game;
            _game.AddPlayer(TestUsers.Mike);
            _game.AddPlayer(TestUsers.Lucas);
            _gameStorage.AddAsync(_game).Wait();
        }

        [Test]
        public void GeneratesTenPointsForEachPlayer()
        {
            var roundAnswersBuilder = new PlayerAnswersBuilder(_game.Id, 1);

            var dustinAnwers = roundAnswersBuilder
                .ForPlayer(TestUsers.Dustin)
                .AddAnswer("Nome", "Bruno")
                .AddAnswer("CEP", "Brasil")
                .AddAnswer("FDS", "Breaking bad")
                .Build();

            var lucasAnswers = roundAnswersBuilder
                .ForPlayer(TestUsers.Lucas)
                .AddAnswer("Nome", "Bruna")
                .AddAnswer("CEP", "Brasilia")
                .AddAnswer("FDS", "Ben 10")
                .Build();

            _answerStorage.AddAsync(dustinAnwers).Wait();
            _answerStorage.AddAsync(lucasAnswers).Wait();            

            var roundValidationsBuilder = new PlayerValidationsBuilder(_game.Id, 1);

            var dustinValidations = roundValidationsBuilder
                .ForPlayer(TestUsers.Dustin)
                .ForTheme("Nome").ValidateAnswers("Bruna")
                .ForTheme("CEP").ValidateAnswers("Brasilia")
                .ForTheme("FDS").ValidateAnswers("Ben 10")
                .Build();

            var lucasValidations = roundValidationsBuilder
                .ForPlayer(TestUsers.Lucas)
                .ForTheme("Nome").ValidateAnswers("Bruno")
                .ForTheme("CEP").ValidateAnswers("Brasil")
                .ForTheme("FDS").ValidateAnswers("Breaking bad")
                .Build();

            _validationStorage.AddAsync(dustinValidations).Wait();
            _validationStorage.AddAsync(lucasValidations).Wait();

            _roundScorer.ProcessRoundPontuationAsync(_game.Id, 1).Wait();
            var playersAnswers = _gamePontuationStorage.GetPontuationsAsync(_game.Id, 1).Result;

            Assert.AreEqual(playersAnswers.GetPlayerPontuation(TestUsers.Dustin.Id), 30);
            Assert.AreEqual(playersAnswers.GetPlayerPontuation(TestUsers.Lucas.Id), 30);
        }
    }

    
}