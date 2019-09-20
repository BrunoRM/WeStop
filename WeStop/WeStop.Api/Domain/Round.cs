using System;

namespace WeStop.Api.Domain
{
    public sealed class Round
    {
        public Round(Guid gameId, int number, string sortedLetter)
        {
            GameId = gameId;
            Number = number;
            SortedLetter = sortedLetter;
        }

        public Guid GameId { get; set; }
        public int Number { get; private set; }
        public string SortedLetter { get; private set; }
    }
}