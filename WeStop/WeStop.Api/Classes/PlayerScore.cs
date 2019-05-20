using System;

namespace WeStop.Api.Classes
{
    public class PlayerScore
    {
        public Guid PlayerId { get; set; }
        public string UserName { get; set; }
        public int RoundPontuation { get; set; }
        public int GamePontuation { get; set; }
    }
}