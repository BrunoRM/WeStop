using System;

namespace WeStop.Api.Classes
{
    public struct PlayerPontuation
    {
        public Guid PlayerId { get; set; }
        public string UserName { get; set; }
        public int Pontuation { get; set; }
    }
}
