using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;

namespace WeStop.Api.Extensions
{
    public static class PlayersRoundExtensions
    {
        public static bool MoreThanOnePlayerReportedAnswerForTheme(this ICollection<PlayerRound> players) =>
            players.Count() > 1;
    }
}
