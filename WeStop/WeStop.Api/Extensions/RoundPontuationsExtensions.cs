using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;

namespace WeStop.Api.Extensions
{
    public static class RoundPontuationsExtensions
    {
        public static void AddPontuationForThemeInRound(this ICollection<RoundPontuations> roundsPontuations, Guid roundId, string theme, int points)
        {
            if (roundsPontuations.Any(rp => rp.RoundId == roundId))
            {
                var roundPontuations = roundsPontuations.First(rp => rp.RoundId == roundId);

                if (!roundPontuations.ThemesPontuations.Any(tp => tp.Theme.Equals(theme)))
                {
                    roundPontuations.AddPontuationForTheme(theme, points);
                }
            }
            else
            {
                roundsPontuations.Add(new RoundPontuations(roundId, new List<ThemePontuation>
                {
                    new ThemePontuation(theme, points)
                }));
            }
        }
    }
}