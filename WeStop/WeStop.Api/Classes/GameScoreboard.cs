using System;
using System.Collections.Generic;

namespace WeStop.Api.Classes
{
    public class GameScoreboard
    {
        public GameScoreboard(Guid gameId, ICollection<PlayerPontuation> playersPontuations)
        {
            GameId = gameId;
            PlayersPontuations = playersPontuations;
        }

        public Guid GameId { get; private set; }
        public ICollection<PlayerPontuation> PlayersPontuations { get; private set; }
    }
}