using System;

namespace WeStop.Api.Classes
{
    public struct RoundScoreboard
    {
        public RoundScoreboard(Guid playerId, string userName, int pontuation)
        {
            this.PlayerId = playerId;
            this.UserName = userName;
            this.Pontuation = pontuation;
        }

        public Guid PlayerId { get; private set; }
        public string UserName { get; private set; }
        public int Pontuation { get; private set; }
    }
}