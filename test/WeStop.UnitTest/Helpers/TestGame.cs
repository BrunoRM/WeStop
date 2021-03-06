using WeStop.Core;

namespace WeStop.UnitTest.Helpers
{
    public static class TestGame
    {
        public static string Name { get; set; }
        public static string Password { get; set; }

        public static GameOptions Options => new GameOptions(new string[] { "Nome", "CEP", "FDS" },
                new string[] { "A", "B", "C" }, 3, 3, 30);

        public static Game Game { get; } = new Game(Name, Password, Options);
    }
}