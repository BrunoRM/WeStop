using System;

namespace WeStop.Api.Dtos
{
    public class CreateGameDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public GameOptionsDto GameOptions { get; set; }
    }

    public class GameOptionsDto
    {
        public string[] Themes { get; set; }
        public string[] AvailableLetters { get; set; }
        public int Rounds { get; set; }
        public int RoundTime { get; set; }
        public int NumberOfPlayers { get; set; }
    }
}