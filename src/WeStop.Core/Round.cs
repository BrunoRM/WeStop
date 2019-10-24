using System;
using System.Collections.Generic;

namespace WeStop.Core
{
    public sealed class Round
    {
        public Round(Guid gameId, int number, string sortedLetter)
        {
            GameId = gameId;
            Number = number;
            SortedLetter = sortedLetter;
            ValidatedThemes = new List<string>();
            Finished = false;
        }

        public Guid GameId { get; set; }
        public int Number { get; private set; }
        public string SortedLetter { get; private set; }
        public string ThemeBeingValidated { get; set; }
        public ICollection<string> ValidatedThemes { get; set; }
        public bool Finished { get; private set; }

        public void Finish() =>
            Finished = true;
    }
}