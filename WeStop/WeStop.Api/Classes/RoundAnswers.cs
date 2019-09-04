using System;
using System.Collections.Generic;

namespace WeStop.Api.Classes
{
    public class RoundAnswers
    {
        public RoundAnswers(Guid roundId, ICollection<ThemeAnswer> answers)
        {
            RoundId = roundId;
            Answers = answers;
        }

        public Guid RoundId { get; set; }
        public ICollection<ThemeAnswer> Answers { get; set; }
    }
}
