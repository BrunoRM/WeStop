using WeStop.Api.Domain;

namespace WeStop.UnitTest
{
    public static class TestGame
    {
        private static GameOptions _gameOptions = new GameOptions(new string[] { "Nome", "CEP", "FDS" },
                new string[] { "A", "B", "C" }, 3, 3, 30);
                
        public static Game CreateGame()
        {
            var game = new Game(TestUsers.Dustin , "TestGame", "", _gameOptions);
            game.AddPlayer(TestUsers.Mike);
            game.AddPlayer(TestUsers.Lucas);
            game.AddPlayer(TestUsers.Will);

            return game;
        }
    }
}