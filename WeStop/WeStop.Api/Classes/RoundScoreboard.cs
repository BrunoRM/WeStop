using System;
using System.Collections.Generic;

namespace WeStop.Api.Classes
{
    public class RoundScoreboard
    {
        public RoundScoreboard(Guid gameId, int roundNumber, ICollection<PlayerPontuation> playersPontuations)
        {
            GameId = gameId;
            RoundNumber = roundNumber;
            PlayersPontuations = playersPontuations;
        }

        public Guid GameId { get; set; }
        public int RoundNumber { get; set; }
        public ICollection<PlayerPontuation> PlayersPontuations { get; private set; }
    }
}