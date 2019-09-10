using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public sealed class RoundPontuations
    {
        private ICollection<ThemePontuation> _themesPontuations;

        public RoundPontuations(Guid gameId, int roundNumber, Guid playerId, ThemePontuation themePontuation)
        {
            RoundNumber = roundNumber;
            _themesPontuations = new List<ThemePontuation>
            {
                themePontuation
            };
        }

        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public int RoundNumber { get; set; }
        public IReadOnlyCollection<ThemePontuation> ThemesPontuations => _themesPontuations.ToList();

        public void AddPontuationForTheme(string theme, int points)
        {
            _themesPontuations.Add(new ThemePontuation(theme, points));
        }

        public int GetTotalPontuation() =>
            ThemesPontuations.Sum(x => x.Pontuation);

        public int GetPontuationForTheme(string theme) =>
            ThemesPontuations.Where(x => x.Theme.Equals(theme)).Sum(x => x.Pontuation);
    }
}
