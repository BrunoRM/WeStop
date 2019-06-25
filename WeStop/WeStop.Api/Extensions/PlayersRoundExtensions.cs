using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;

namespace WeStop.Api.Extensions
{
    public static class PlayersRoundExtensions
    {
        public static bool MoreThanOnePlayerRepliedAnswerForTheme(this ICollection<PlayerRound> playersInRound) =>
            playersInRound.Count() > 1;
    }
}
