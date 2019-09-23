using NUnit.Framework;
using WeStop.Api.Domain.Services;
using WeStop.Api.Managers;
using WeStop.UnitTest.Extensions;
using WeStop.UnitTest.Helpers;

namespace WeStop.UnitTest
{
    [TestFixture]
    public class RoundScorerTests : Test
    {
        [SetUp]
        public void Setup()
        {
            CreateDefaultConfig();
        }

        [Test]
        public void GivenDistinctsAnswersShouldGiveTenPointsForEach()
        {
            Game.StartNextRound();
            var roundAnswersBuilder = new PlayerAnswersBuilder(Game);

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

            GameManager.AddRoundAnswersAsync(dustinAnwers).Wait();
            GameManager.AddRoundAnswersAsync(lucasAnswers).Wait();           

            var roundValidationsBuilder = new PlayerValidationsBuilder(Game);

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

            GameManager.AddRoundValidationsAsync(dustinValidations).Wait();
            GameManager.AddRoundValidationsAsync(lucasValidations).Wait();

            RoundScorer.ProcessCurrentRoundPontuationAsync(Game.Id).Wait();
            
            var roundPontuations = PontuationStorage.GetPontuationsAsync(Game.Id, Game.CurrentRoundNumber).Result;

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
            Game.StartNextRound();
            var roundAnswersBuilder = new PlayerAnswersBuilder(Game);

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

            GameManager.AddRoundAnswersAsync(dustinAnwers).Wait();
            GameManager.AddRoundAnswersAsync(lucasAnswers).Wait();                   

            var roundValidationsBuilder = new PlayerValidationsBuilder(Game);

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

            GameManager.AddRoundValidationsAsync(dustinValidations).Wait();
            GameManager.AddRoundValidationsAsync(lucasValidations).Wait();

            RoundScorer.ProcessCurrentRoundPontuationAsync(Game.Id).Wait();
            var roundPontuations = PontuationStorage.GetPontuationsAsync(Game.Id, Game.CurrentRoundNumber).Result;

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
            Game.StartNextRound();
            var roundAnswersBuilder = new PlayerAnswersBuilder(Game);

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

            GameManager.AddRoundAnswersAsync(dustinAnwers).Wait();
            GameManager.AddRoundAnswersAsync(lucasAnswers).Wait();

            RoundScorer.ProcessCurrentRoundPontuationAsync(Game.Id).Wait();
            var roundPontuations = PontuationStorage.GetPontuationsAsync(Game.Id, Game.CurrentRoundNumber).Result;

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
        public void PontuationIsCorrectWhenAllAnswersDisctinct()
        {
            Game.StartNextRound();
            var roundAnswersBuilder = new PlayerAnswersBuilder(Game);

            var dustinAnwers = roundAnswersBuilder
                .ForPlayer(TestUsers.Dustin)
                .AddAnswer("Nome", "Bruno")
                .AddAnswer("CEP", "Brasil")
                .AddAnswer("FDS", "Breaking Bad")
                .Build();

            var lucasAnswers = roundAnswersBuilder
                .ForPlayer(TestUsers.Lucas)
                .AddAnswer("Nome", "Bruna")
                .AddAnswer("CEP", "Brasilia")
                .AddAnswer("FDS", "Ben 10")
                .Build();

            GameManager.AddRoundAnswersAsync(dustinAnwers).Wait();
            GameManager.AddRoundAnswersAsync(lucasAnswers).Wait();           

            var roundValidationsBuilder = new PlayerValidationsBuilder(Game);

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

            GameManager.AddRoundValidationsAsync(dustinValidations).Wait();
            GameManager.AddRoundValidationsAsync(lucasValidations).Wait();

            RoundScorer.ProcessCurrentRoundPontuationAsync(Game.Id).Wait();
            var roundPontuations = PontuationStorage.GetPontuationsAsync(Game.Id, Game.CurrentRoundNumber).Result;
            
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
        public void GivenInvalidatedAnswersShouldGiveZeroPointsForEach()
        {
            Game.StartNextRound();
            var roundAnswersBuilder = new PlayerAnswersBuilder(Game);

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

            GameManager.AddRoundAnswersAsync(dustinAnwers).Wait();
            GameManager.AddRoundAnswersAsync(lucasAnswers).Wait();           

            var roundValidationsBuilder = new PlayerValidationsBuilder(Game);

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

            GameManager.AddRoundValidationsAsync(dustinValidations).Wait();
            GameManager.AddRoundValidationsAsync(lucasValidations).Wait();

            RoundScorer.ProcessCurrentRoundPontuationAsync(Game.Id).Wait();
            var roundPontuations = PontuationStorage.GetPontuationsAsync(Game.Id, Game.CurrentRoundNumber).Result;
            
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