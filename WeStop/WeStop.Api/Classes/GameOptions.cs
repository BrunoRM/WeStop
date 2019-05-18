namespace WeStop.Api.Classes
{
    public class GameOptions
    {
        public GameOptions(string[] themes, string[] availableLetters, int rounds, int numberOfPlayers)
        {
            Themes = themes;
            AvailableLetters = availableLetters;
            Rounds = rounds;
            NumberOfPlayers = numberOfPlayers;
        }

        public string[] Themes { get; private set; }
        public string[] AvailableLetters { get; private set; }
        public int Rounds { get; private set; }
        public int NumberOfPlayers { get; private set; }
    }
}