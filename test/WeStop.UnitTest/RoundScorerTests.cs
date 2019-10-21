using System.Threading.Tasks;
using NUnit.Framework;
using WeStop.Api.Core.Services;
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
            Game.Players.PutAllPlayersInReadyState();
        }

        [Test]
        public async Task GivenDistinctsAnswersShouldGiveTenPointsForEach()
        {
            await GameManager.StartRoundAsync(Game.Id, (async r =>
            {
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

                await GameManager.AddRoundAnswersAsync(dustinAnwers);
                await GameManager.AddRoundAnswersAsync(lucasAnswers);

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

                await GameManager.AddRoundValidationsAsync(dustinValidations);
                await GameManager.AddRoundValidationsAsync(lucasValidations);

                await RoundScorer.ProcessRoundPontuationAsync((Api.Core.Round)Game.CurrentRound);
                var roundPontuations = Game.GetScoreboard(Game.CurrentRoundNumber);

                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "Nome"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "CEP"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "FDS"));
                Assert.AreEqual(30, roundPontuations.GetPlayerPontuation(TestUsers.Dustin));

                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "Nome"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "CEP"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "FDS"));
                Assert.AreEqual(30, roundPontuations.GetPlayerPontuation(TestUsers.Lucas));
            }));
        }

        [Test]
        public async Task GivenSameAnswersShouldGiveFivePointsForEach()
        {
            await GameManager.StartRoundAsync(Game.Id, (async r =>
            {
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

                await GameManager.AddRoundAnswersAsync(dustinAnwers);
                await GameManager.AddRoundAnswersAsync(lucasAnswers);

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

                await GameManager.AddRoundValidationsAsync(dustinValidations);
                await GameManager.AddRoundValidationsAsync(lucasValidations);

                await RoundScorer.ProcessRoundPontuationAsync(Game.CurrentRound);
                var roundPontuations = Game.GetScoreboard(Game.CurrentRoundNumber);

                Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "Nome"));
                Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "CEP"));
                Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "FDS"));
                Assert.AreEqual(15, roundPontuations.GetPlayerPontuation(TestUsers.Dustin));

                Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "Nome"));
                Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "CEP"));
                Assert.AreEqual(5, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "FDS"));
                Assert.AreEqual(15, roundPontuations.GetPlayerPontuation(TestUsers.Lucas));
            })); 
        }

        [Test]
        public async Task GivenBlankOrNullAnswersShouldGiveZeroPointsForEach()
        {
            await GameManager.StartRoundAsync(Game.Id, (async r =>
            {
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

                await GameManager.AddRoundAnswersAsync(dustinAnwers);
                await GameManager.AddRoundAnswersAsync(lucasAnswers);

                await RoundScorer.ProcessRoundPontuationAsync(Game.CurrentRound);
                var roundPontuations = Game.GetScoreboard(Game.CurrentRoundNumber);

                Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "Nome"));
                Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "CEP"));
                Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "FDS"));
                Assert.AreEqual(0, roundPontuations.GetPlayerPontuation(TestUsers.Dustin));

                Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "Nome"));
                Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "CEP"));
                Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "FDS"));
                Assert.AreEqual(0, roundPontuations.GetPlayerPontuation(TestUsers.Lucas));
            }));
        }

        [Test]
        public async Task PontuationIsCorrectWhenAllAnswersDisctinct()
        {
            await GameManager.StartRoundAsync(Game.Id, (async r =>
            {
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

                await GameManager.AddRoundAnswersAsync(dustinAnwers);
                await GameManager.AddRoundAnswersAsync(lucasAnswers);

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

                await GameManager.AddRoundValidationsAsync(dustinValidations);
                await GameManager.AddRoundValidationsAsync(lucasValidations);

                await RoundScorer.ProcessRoundPontuationAsync(Game.CurrentRound);
                var roundPontuations = Game.GetScoreboard(Game.CurrentRoundNumber);

                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "Nome"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "CEP"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "FDS"));
                Assert.AreEqual(30, roundPontuations.GetPlayerPontuation(TestUsers.Dustin));

                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "Nome"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "CEP"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "FDS"));
                Assert.AreEqual(30, roundPontuations.GetPlayerPontuation(TestUsers.Lucas));
            }));
        }

        [Test]
        public async Task GivenInvalidatedAnswersShouldGiveZeroPointsForEach()
        {
            await GameManager.StartRoundAsync(Game.Id, (async r =>
            {
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

                await GameManager.AddRoundAnswersAsync(dustinAnwers);
                await GameManager.AddRoundAnswersAsync(lucasAnswers);

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

                await GameManager.AddRoundValidationsAsync(dustinValidations);
                await GameManager.AddRoundValidationsAsync(lucasValidations);

                await RoundScorer.ProcessRoundPontuationAsync(Game.CurrentRound);
                var roundPontuations = Game.GetScoreboard(Game.CurrentRoundNumber);

                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "Nome"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "CEP"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Dustin, "FDS"));
                Assert.AreEqual(30, roundPontuations.GetPlayerPontuation(TestUsers.Dustin));

                Assert.AreEqual(10, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "Nome"));
                Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "CEP"));
                Assert.AreEqual(0, roundPontuations.GetPlayerPontuationForTheme(TestUsers.Lucas, "FDS"));
                Assert.AreEqual(10, roundPontuations.GetPlayerPontuation(TestUsers.Lucas));
            }));
        }
    }    
}