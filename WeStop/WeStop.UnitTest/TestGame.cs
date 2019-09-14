using WeStop.Api.Domain;

namespace WeStop.UnitTest
{
    public static class TestGame
    {
        private static GameOptions _gameOptions = new GameOptions(new string[] { "Nome", "CEP", "FDS" },
                new string[] { "A", "B", "C" }, 3, 3, 30);

        public static Game Game = new Game(TestUsers.Dustin , "TestGame", "", _gameOptions);
    }
}