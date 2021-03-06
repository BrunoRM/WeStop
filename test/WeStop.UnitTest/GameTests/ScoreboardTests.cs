using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WeStop.UnitTest.Extensions;
using WeStop.UnitTest.Helpers;

namespace WeStop.UnitTest.GameTests
{
    [TestFixture]
    public class ScoreboardTests : Test
    {
        [SetUp]
        public void Initialize()
        {
            CreateDefaultConfig();
            Game.Players.PutAllPlayersInReadyState();
        }

        [Test]
        public async Task ShouldReturnPontuationsOnlyForPlayersInRound()
        {
            await GameManager.StartRoundAsync(Game.Id, async (r, pIds) =>
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

                await RoundScorer.ProcessRoundPontuationAsync(Game.CurrentRound);

                var scoreBoard = Game.GetScoreboard(Game.CurrentRoundNumber);

                Assert.True(scoreBoard.Any(p => p.PlayerId == TestUsers.Dustin.Id));
                Assert.True(scoreBoard.Any(p => p.PlayerId == TestUsers.Lucas.Id));
                Assert.False(scoreBoard.Any(p => p.PlayerId == TestUsers.Mike.Id));
                Assert.False(scoreBoard.Any(p => p.PlayerId == TestUsers.Will.Id));
            });
        }
    }
}