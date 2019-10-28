using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Core.Extensions
{
    public static class RoundAnswersExtensions
    {
        public static IEnumerable<Guid> GetPlayersIdsThatRepliedAnswer(this IEnumerable<RoundAnswers> roundAnswers, Answer answer) =>
            roundAnswers.SelectMany(ra => ra.Answers.Where(a => a == answer).Select(a => ra.PlayerId)).ToList();

        public static IEnumerable<Guid> GetPlayersIdsWithBlankAnswersForTheme(this IEnumerable<RoundAnswers> roundAnswers, string theme) =>
            roundAnswers.SelectMany(ra => ra.Answers.Where(a => a.Theme.Equals(theme) && a.IsEmpty()).Select(a => ra.PlayerId)).ToList();

        public static Answer[] GetAnswersOfTheme(this IEnumerable<RoundAnswers> roundAnswers, string theme) =>
            roundAnswers.SelectMany(ra => ra.Answers.Where(a => a.Theme.Equals(theme) && !a.IsEmpty())).Distinct().ToArray();

        public static IEnumerable<Validation> BuildValidationsForPlayer(this IEnumerable<RoundAnswers> roundsAnswers, Guid playerId, string theme, string sortedLetter)
        {
            var answers = roundsAnswers.Where(ra => ra.PlayerId != playerId).SelectMany(ra => ra.Answers.Where(a => a.Theme.Equals(theme) && !a.IsEmpty())).Distinct().ToList();
            
            foreach (var answer in answers)
            {
                if (answer.StartsWith(sortedLetter))
                    yield return new Validation(answer, true);
                else
                    yield return new Validation(answer, false);
            }
        }

        public static int GetTotalThemesForPlayerValidate(this IEnumerable<RoundAnswers> roundAnswers, Guid playerId, int roundNumber)
        {
            return roundAnswers.Where(ra => ra.RoundNumber == roundNumber && ra.PlayerId != playerId)
                .SelectMany(ra => ra.Answers.Where(a => !a.IsEmpty()).Select(a => a.Theme))
                .Distinct().Count();
        }
    }
}