using System;

namespace WeStop.Api.Domain
{
    public sealed class Round
    {
        public Round(int number, string sortedLetter)
        {
            Number = number;
            SortedLetter = sortedLetter;
        }

        public int Number { get; private set; }
        public string SortedLetter { get; private set; }
    }
}