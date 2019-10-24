using System.Collections.Generic;
using WeStop.Core;

namespace WeStop.UnitTest.Extensions
{
    public static class PlayersExtensions
    {
        public static void PutAllPlayersInReadyState(this ICollection<Player> players)
        {
            foreach (var player in players)
            {
                player.IsReady = true;
            }
        }
    }
}
