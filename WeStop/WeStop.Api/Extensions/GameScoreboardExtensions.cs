using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;

namespace WeStop.Api.Extensions
{
    public static class GameScoreboardExtensions
    {
        public static IEnumerable<PlayerPontuation> GetWinners(this GameScoreboard gameScoreboard)
        {
            int playersBestPontuation = gameScoreboard.PlayersPontuations.Max(pp => pp.Pontuation);

            return gameScoreboard.PlayersPontuations.Where(p => p.Pontuation == playersBestPontuation)
                .OrderBy(p => p.UserName);
        }
    }
}
