using System;

namespace WeStop.Api.Dtos
{
    public class GameSummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public string State { get; set; }
        public int NumberOfPlayers { get; set; }
        public int MaxNumberOfPlayers { get; set; }
        public int NumberOfRounds { get; set; }
        public int Time { get; set; }
        public string[] Themes { get; set; }
        public int CurrentRoundNumber { get; set; }
        public string[] AvailableLetters { get; set; }
        public string[] SortedLetters { get; set; }
    }
}