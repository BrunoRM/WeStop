using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Domain
{
    public sealed class RoundPontuations
    {
        public RoundPontuations(Guid gameId, int roundNumber, Guid playerId, ThemePontuation themePontuation)
        {
            PlayerId = playerId;
            GameId = gameId;
            RoundNumber = roundNumber;
            ThemesPontuations = new List<ThemePontuation>
            {
                themePontuation
            };
        }

        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public int RoundNumber { get; set; }
        public ICollection<ThemePontuation> ThemesPontuations { get; set; }
        public int TotalPontuation => ThemesPontuations.Sum(x => x.Pontuation);

        public int this[string theme]
        {
            get
            {
                return ThemesPontuations.FirstOrDefault(tp => tp.Theme.Equals(theme))?.Pontuation ?? 0;
            }
        }

        public void AddPontuationForTheme(string theme, int points)
        {
            ThemesPontuations.Add(new ThemePontuation(theme, points));
        }
    }
}
