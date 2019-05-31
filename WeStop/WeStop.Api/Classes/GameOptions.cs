using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public class GameOptions
    {
        public GameOptions(string[] themes, string[] availableLetters, int rounds, int numberOfPlayers)
        {
            Themes = themes;
            AvailableLetters = new Dictionary<string, bool>(availableLetters.Select(al => new KeyValuePair<string, bool>(al, false)));
            Rounds = rounds;
            NumberOfPlayers = numberOfPlayers;
        }

        public string[] Themes { get; private set; }
        public IDictionary<string, bool> AvailableLetters { get; set; }
        public int Rounds { get; private set; }
        public int NumberOfPlayers { get; private set; }
    }
}