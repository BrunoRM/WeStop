using System.Collections.Generic;

namespace WeStop.Api.Domain
{
    public struct RoundScoreboard
    {
        public ICollection<PlayerPontuation> Pontuations { get; set; }
    }

    public struct PlayerPontuation
    {
        public string UserName { get; set; }
        public int RoundPontuation { get; set; }
        public int GamePontuation { get; set; }
    }
}
