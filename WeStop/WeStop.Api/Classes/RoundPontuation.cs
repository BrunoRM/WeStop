using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public class RoundPontuation
    {
        public RoundPontuation(Guid roundId, ICollection<ThemePontuation> themesPontuations)
        {
            RoundId = roundId;
            ThemesPontuations = themesPontuations;
        }

        public Guid RoundId { get; set; }
        public ICollection<ThemePontuation> ThemesPontuations { get; set; }

        public int GetTotalPontuation() =>
            ThemesPontuations.Sum(x => x.Pontuation);
    }
}
