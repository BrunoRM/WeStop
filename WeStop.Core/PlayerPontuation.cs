using System;
using System.Collections.Generic;

namespace WeStop.Core
{
    public class PlayerPontuation
    {
        public PlayerPontuation()
        {
            Pontuations = new List<ThemePontuation>();
        }

        public Guid PlayerId { get; set; }
        public string UserName { get; set; }
        public int RoundPontuation { get; set; }
        public int GamePontuation { get; set; }
        public ICollection<ThemePontuation> Pontuations { get; set; }
    }
}
