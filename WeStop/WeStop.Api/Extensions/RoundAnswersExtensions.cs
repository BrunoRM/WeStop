using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;

namespace WeStop.Api.Extensions
{
    public static class RoundAnswersExtensions
    {
        public static void AddThemeAnswerForRound(this ICollection<RoundAnswers> roundsAnswers, Guid roundId, string theme, string answer)
        {
            if (roundsAnswers.Any(ra => ra.RoundNumber == roundId))
            {
                var roundAnswers = roundsAnswers.First(ra => ra.RoundNumber == roundId);

                if (!roundAnswers.Answers.Any(a => a.Theme.Equals(theme)))
                    roundAnswers.Answers.Add(new ThemeAnswer(theme, answer));
            }
            else
            {
                roundsAnswers.Add(new RoundAnswers(roundId, new List<ThemeAnswer> 
                { 
                    new ThemeAnswer(theme, answer) 
                }));
            }
        }
    }
}