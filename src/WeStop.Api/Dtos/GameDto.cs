using System;
using System.Collections.Generic;

namespace WeStop.Api.Dtos
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public int MaxNumberOfPlayers { get; set; }
        public int NumberOfRounds { get; set; }
        public int Time { get; set; }
        public string[] Themes { get; set; }
        public int NextRoundNumber { get; set; }
        public ICollection<PlayerDto> Players { get; set; }
        public ICollection<PlayerScoreDto> ScoreBoard { get; set; }
    }
}
