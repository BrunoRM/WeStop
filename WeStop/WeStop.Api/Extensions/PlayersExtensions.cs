using System.Collections.Generic;
using WeStop.Api.Domain;

namespace WeStop.Api.Extensions
{
    public static class PlayersExtensions
    {
        public static IEnumerable<Player> PutReadyPlayersInRound(this ICollection<Player> players)
        {
            foreach (var player in players)
            {
                if (player.IsReady)
                {
                    player.InRound = true;
                    yield return player;
                }
            }
        }

        public static IEnumerable<Player> PutAllPlayersInWaiting(this ICollection<Player> players)
        {
            foreach (var player in players)
            {
                player.InRound = false;
                player.IsReady = false;
                yield return player;
            }
        }
    }
}
