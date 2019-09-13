using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Domain;

namespace WeStop.Api.Extensions
{
    public static class RoundAnswersExtensions
    {
        public static IEnumerable<Guid> GetPlayersIdsThatRepliedAnswer(this IEnumerable<RoundAnswers> roundAnswers, Answer answer) =>
            roundAnswers.SelectMany(ra => ra.Answers.Where(a => a == answer).Select(a => ra.PlayerId));

        public static IEnumerable<Guid> GetPlayersIdsWithBlankAnswers(this IEnumerable<RoundAnswers> roundAnswers) =>
            roundAnswers.SelectMany(ra => ra.Answers.Where(a => a.Value.Equals(string.Empty)).Select(a => ra.PlayerId));

        public static Answer[] GetAnswersOfTheme(this IEnumerable<RoundAnswers> roundAnswers, string theme) =>
            roundAnswers.SelectMany(ra => ra.Answers.Where(a => a.Theme.Equals(theme))).Distinct().ToArray();

        public static IEnumerable<Validation> BuildValidationsForPlayer(this IEnumerable<RoundAnswers> roundsAnswers, Guid playerId)
        {
            var themes = roundsAnswers.SelectMany(ra => ra.Answers.Select(a => a.Theme));

            foreach (var theme in themes)
            {
                var answers = roundsAnswers.Where(ra => ra.PlayerId != playerId).SelectMany(ra => ra.Answers);
                foreach (var answer in answers)
                {
                    yield return new Validation(answer);
                }
            }
        }
    }
}