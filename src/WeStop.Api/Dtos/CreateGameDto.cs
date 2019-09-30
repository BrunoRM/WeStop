using System;
using WeStop.Api.Domain;

namespace WeStop.Api.Dtos
{
    public class CreateGameDto
    {
        public User User { get; set; }
        public string Name { get; set; }
        public GameOptions GameOptions { get; set; }
    }

    public class GameOptionsDto
    {
        public string[] Themes { get; set; }
        public string[] AvailableLetters { get; set; }
        public int Rounds { get; set; }
        public int Time { get; set; }
        public int NumberOfPlayers { get; set; }
    }
}