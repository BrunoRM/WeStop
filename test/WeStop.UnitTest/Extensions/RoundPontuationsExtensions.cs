using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Domain;

namespace WeStop.UnitTest.Extensions
{
    public static class RoundPontuationsExtensions
    {
        public static int GetPlayerPontuation(this IEnumerable<RoundPontuations> roundPontuations, User user) =>
            roundPontuations.FirstOrDefault(x => x.PlayerId == user.Id).TotalPontuation;

        public static int GetPlayerPontuationForTheme(this IEnumerable<RoundPontuations> roundPontuations, User user, string theme) =>
            roundPontuations.FirstOrDefault(x => x.PlayerId == user.Id)[theme];
    }
}