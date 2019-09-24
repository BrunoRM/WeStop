using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Domain
{
    public class GameOptions
    {
        public GameOptions(string[] themes, string[] availableLetters, int rounds, int numberOfPlayers, int roundTime)
        {
            Themes = themes;
            AvailableLetters = new Dictionary<string, bool>(availableLetters.Select(al => new KeyValuePair<string, bool>(al, false)));
            Rounds = rounds;
            NumberOfPlayers = numberOfPlayers;
            RoundTime = roundTime;
        }

        public string[] Themes { get; private set; }
        public IDictionary<string, bool> AvailableLetters { get; set; }
        public int Rounds { get; private set; }
        public int RoundTime { get; private set; }
        public int NumberOfPlayers { get; private set; }
    }
}