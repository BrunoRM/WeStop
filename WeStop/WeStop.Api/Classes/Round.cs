using System;

namespace WeStop.Api.Classes
{
    public sealed class Round
    {
        public Round(int number, string sortedLetter)
        {
            Number = number;
            SortedLetter = sortedLetter;
            Finished = false;
        }

        public int Number { get; private set; }
        public string SortedLetter { get; private set; }
        public bool Finished { get; private set; }
    }
}