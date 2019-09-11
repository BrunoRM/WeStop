using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;

namespace WeStop.Api.Extensions
{
    public static class RoundAnswersExtensions
    {
        public static IEnumerable<ThemeValidation> BuildValidationsForPlayer(this ICollection<RoundAnswers> roundsAnswers, Guid playerId)
        {
            var themes = roundsAnswers.SelectMany(ra => ra.Answers.Select(a => a.Theme));

            foreach (var theme in themes)
            {
                var answers = roundsAnswers.Where(ra => ra.PlayerId != playerId).SelectMany(ra => ra.Answers.Select(a => a.Value));
                yield return new ThemeValidation(theme, answers.Select(a => new AnswerValidation(a, true)).ToList());
            }
        }
    }
}