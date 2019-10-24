using System.Collections.Generic;
using System.Linq;
using WeStop.Core;

namespace WeStop.UnitTest.Extensions
{
    public static class PlayerPontuationExtensions
    {
        public static int GetPlayerPontuation(this IEnumerable<PlayerPontuation> roundPontuations, User user) =>
            roundPontuations.FirstOrDefault(x => x.PlayerId == user.Id).RoundPontuation;

        public static int GetPlayerPontuationForTheme(this IEnumerable<PlayerPontuation> playersPontuations, User user, string theme) =>
            playersPontuations.First(p => p.PlayerId == user.Id).Pontuations.First(p => p.Theme.Equals(theme)).Pontuation;
    }
}