using System;

namespace WeStop.Api.Classes
{
    public class PlayerFinalPontuation
    {
        public Guid PlayerId { get; set; }
        public string UserName { get; set; }
        public int Pontuation { get; set; }
    }
}