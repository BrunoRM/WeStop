using NUnit.Framework;
using WeStop.Api.Domain.Services;
using WeStop.Api.Infra.Storages.InMemory;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.UnitTest.Extensions;
using WeStop.UnitTest.Helpers;

namespace WeStop.UnitTest
{
    [TestFixture]
    public class RoundScorerTests
    {
        private IGameStorage _gameStorage;
        private IAnswerStorage _answerStorage;
        private IValidationStorage _validationStorage;
        private IPontuationStorage _gamePontuationStorage;
        private RoundScorer _roundScorerManager;

        [SetUp]
        public void Setup()
        {
            _gameStorage = new GameStorage();
            _answerStorage = new AnswerStorage();
            _validationStorage = new ValidationStorage();
            _gamePontuationStorage = new PontuationStorage();
            _roundScorerManager = new RoundScorer(_gameStorage, _answerStorage, _validationStorage, _gamePontuationStorage);
        }

        [Test]
        public void GivenDistinctsAnswersShouldGiveTenPointsForEach()
        {
            var roundAnswersBuilder = new PlayerAnswersBuilder(TestGame.Game);

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

            var roundValidationsBuilder = new PlayerValidationsBuilder(TestGame.Game);

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

            _roundScorerManager.ProcessCurrentRoundPontuationAsync(TestGame.Game.Id).Wait();
            var roundPontuations = _gamePontuationStorage.GetPontuationsAsync(TestGame.Game.Id, 1).Result;

            Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "Nome"));
            Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "CEP"));
            Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "FDS"));
            Assert.AreEqual(30, roundPontuations.GetPlayerPontuation(TestUsers.Dustin));

            Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "Nome"));
            Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "CEP"));
            Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "FDS"));
            Assert.AreEqual(30, roundPontuations.GetPlayerPontuation(TestUsers.Lucas));
        }

        [Test]
        public void GivenSameAnswersShouldGiveFivePointsForEach()
        {
            var roundAnswersBuilder = new PlayerAnswersBuilder(TestGame.Game);

            var dustinAnwers = roundAnswersBuilder
                .ForPlayer(TestUsers.Dustin)
                .AddAnswer("Nome", "Bruno")
                .AddAnswer("CEP", "Brasil")
                .AddAnswer("FDS", "Breaking bad")
                .Build();

            var lucasAnswers = roundAnswersBuilder
                .ForPlayer(TestUsers.Lucas)
                .AddAnswer("Nome", "Bruno")
                .AddAnswer("CEP", "Brasil")
                .AddAnswer("FDS", "Breaking bad")
                .Build();

            _answerStorage.AddAsync(dustinAnwers).Wait();
            _answerStorage.AddAsync(lucasAnswers).Wait();            

            var roundValidationsBuilder = new PlayerValidationsBuilder(TestGame.Game);

            var dustinValidations = roundValidationsBuilder
                .ForPlayer(TestUsers.Dustin)
                .ForTheme("Nome").ValidateAnswers("Bruno")
                .ForTheme("CEP").ValidateAnswers("Brasil")
                .ForTheme("FDS").ValidateAnswers("Breaking bad")
                .Build();

            var lucasValidations = roundValidationsBuilder
                .ForPlayer(TestUsers.Lucas)
                .ForTheme("Nome").ValidateAnswers("Bruno")
                .ForTheme("CEP").ValidateAnswers("Brasil")
                .ForTheme("FDS").ValidateAnswers("Breaking bad")
                .Build();

            _validationStorage.AddAsync(dustinValidations).Wait();
            _validationStorage.AddAsync(lucasValidations).Wait();

            _roundScorerManager.ProcessCurrentRoundPontuationAsync(TestGame.Game.Id).Wait();
            var roundPontuations = _gamePontuationStorage.GetPontuationsAsync(TestGame.Game.Id, 1).Result;

            Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "Nome"));
            Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "CEP"));
            Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "FDS"));
            Assert.AreEqual(15, roundPontuations.GetPlayerPontuation(TestUsers.Dustin));

            Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "Nome"));
            Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "CEP"));
            Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "FDS"));
            Assert.AreEqual(15, roundPontuations.GetPlayerPontuation(TestUsers.Lucas));
        }

        [Test]
        public void GivenBlankOrNullAnswersShouldGiveZeroPointsForEach()
        {
            var roundAnswersBuilder = new PlayerAnswersBuilder(TestGame.Game);

            var dustinAnwers = roundAnswersBuilder
                .ForPlayer(TestUsers.Dustin)
                .AddAnswer("Nome", "")
                .AddAnswer("CEP", "")
                .Build();

            var lucasAnswers = roundAnswersBuilder
                .ForPlayer(TestUsers.Lucas)
                .AddAnswer("CEP", "")
                .AddAnswer("FDS", "")
                .Build();

            _answerStorage.AddAsync(dustinAnwers).Wait();
            _answerStorage.AddAsync(lucasAnswers).Wait();            

            _roundScorerManager.ProcessCurrentRoundPontuationAsync(TestGame.Game.Id).Wait();
            var roundPontuations = _gamePontuationStorage.GetPontuationsAsync(TestGame.Game.Id, 1).Result;

            Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "Nome"));
            Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "CEP"));
            Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "FDS"));
            Assert.AreEqual(0, roundPontuations.GetPlayerPontuation(TestUsers.Dustin));

            Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "Nome"));
            Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "CEP"));
            Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "FDS"));
            Assert.AreEqual(0, roundPontuations.GetPlayerPontuation(TestUsers.Lucas));
        }

        [Test]
        public void GivenInvalidatedAnswersShouldGiveZeroPointsForEach()
        {
            var roundAnswersBuilder = new PlayerAnswersBuilder(TestGame.Game);

            var dustinAnwers = roundAnswersBuilder
                .ForPlayer(TestUsers.Dustin)
                .AddAnswer("Nome", "Bruno")
                .AddAnswer("CEP", "Brasil")
                .AddAnswer("FDS", "Breaking Bad")
                .Build();

            var lucasAnswers = roundAnswersBuilder
                .ForPlayer(TestUsers.Lucas)
                .AddAnswer("Nome", "Bruna")
                .AddAnswer("CEP", "Br")
                .AddAnswer("FDS", "Ben")
                .Build();

            _answerStorage.AddAsync(dustinAnwers).Wait();
            _answerStorage.AddAsync(lucasAnswers).Wait();            

            var roundValidationsBuilder = new PlayerValidationsBuilder(TestGame.Game);

            var dustinValidations = roundValidationsBuilder
                .ForPlayer(TestUsers.Dustin)
                .ForTheme("Nome").ValidateAnswers("Bruna")
                .ForTheme("CEP").InvalidateAnswers("Br")
                .ForTheme("FDS").InvalidateAnswers("Ben")
                .Build();

            var lucasValidations = roundValidationsBuilder
                .ForPlayer(TestUsers.Lucas)
                .ForTheme("Nome").ValidateAnswers("Bruno")
                .ForTheme("CEP").ValidateAnswers("Brasil")
                .ForTheme("FDS").ValidateAnswers("Breaking bad")
                .Build();

            _validationStorage.AddAsync(dustinValidations).Wait();
            _validationStorage.AddAsync(lucasValidations).Wait();

            _roundScorerManager.ProcessCurrentRoundPontuationAsync(TestGame.Game.Id).Wait();
            var roundPontuations = _gamePontuationStorage.GetPontuationsAsync(TestGame.Game.Id, TestGame.Game.CurrentRound.Number).Result;
            
            Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "Nome"));
            Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "CEP"));
            Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "FDS"));
            Assert.AreEqual(30, roundPontuations.GetPlayerPontuation(TestUsers.Dustin));

            Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "Nome"));
            Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "CEP"));
            Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "FDS"));
            Assert.AreEqual(10, roundPontuations.GetPlayerPontuation(TestUsers.Lucas));
        }
    }

    
}