using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public class Round
    {
        public Round(int number, string sortedLetter)
        {
            Number = number;
            SortedLetter = sortedLetter;
            Finished = false;
        }

        public Guid Id { get; set; }
        public int Number { get; private set; }
        public string SortedLetter { get; private set; }
        public bool Finished { get; private set; }
    }
}