using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public class RoundPontuations
    {
        private ICollection<ThemePontuation> _themesPontuations;

        public RoundPontuations(Guid roundId, ICollection<ThemePontuation> themesPontuations)
        {
            RoundId = roundId;
            _themesPontuations = themesPontuations;
        }

        public Guid RoundId { get; set; }
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
