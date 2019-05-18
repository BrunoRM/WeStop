using System.Collections.Generic;

namespace WeStop.Api.Classes
{
    public class FinalScoreboard
    {
        public string Winner { get; set; }
        public ICollection<PlayerFinalPontuation> PlayersPontuations { get; set; }
    }
}