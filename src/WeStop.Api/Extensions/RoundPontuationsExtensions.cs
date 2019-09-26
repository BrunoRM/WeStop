﻿using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Domain;

namespace WeStop.Api.Extensions
{
    public static class RoundPontuationsExtensions
    {
        public static Guid[] GetWinners(this IEnumerable<RoundPontuations> pontuations)
        {
            int playersBestPontuation = pontuations.Max(pp => pp.ThemesPontuations.Sum(tp => tp.Pontuation));

            return pontuations.Where(p => p.ThemesPontuations.Sum(tp => tp.Pontuation) == playersBestPontuation)
                .OrderBy(p => p.PlayerId)
                .Select(p => p.PlayerId).ToArray();
        }
    }
}