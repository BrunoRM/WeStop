using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Domain;

namespace WeStop.UnitTest.Extensions
{
    public static class RoundPontuationsExtensions
    {
        public static int GetPlayerPontuation(this IEnumerable<RoundPontuations> roundAnswers, Guid playerId) =>
            roundAnswers.FirstOrDefault(x => x.PlayerId == playerId).TotalPontuation;
    }
}