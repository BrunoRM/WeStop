using System;

namespace WeStop.Api.Classes
{
    public class PlayerPontuation
    {
        public Guid PlayerId { get; private set; }
        public string UserName { get; private set; }
        public int Pontuation { get; private set; }
    }
}
