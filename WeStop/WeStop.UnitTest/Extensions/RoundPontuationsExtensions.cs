using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Domain;

namespace WeStop.UnitTest.Extensions
{
    public static class RoundPontuationsExtensions
    {
        public static int GetPlayerPontuation(this IEnumerable<RoundPontuations> roundAnswers, User user) =>
            roundAnswers.FirstOrDefault(x => x.PlayerId == user.Id).TotalPontuation;

        public static int GetPlayerPontuationForTheme(this IEnumerable<RoundPontuations> roundAnswers, User user, string theme) =>
            roundAnswers.FirstOrDefault(x => x.PlayerId == user.Id)[theme];
    }
}