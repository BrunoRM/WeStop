using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;

namespace WeStop.Api.Extensions
{
    public static class PlayersExtensions
    {
        public static string[] GetRoundDistinctsAnswersByTheme(this IEnumerable<Player> players, Guid roundId, string theme)
        {
            var answers = players
                .SelectMany(p => p.RoundsAnswers
                    .Where(ra => ra.RoundId == roundId)
                    .SelectMany(ra => ra.Answers
                        .Where(a => a.Theme.Equals(theme)))
                        .Select(ta => ta.Answer.Trim()))
                .Distinct().ToArray();

            return answers;
        }        

        public static int GetRoundValidVotesCountForThemeAnswer(this IEnumerable<Player> players, Guid roundId, string theme, string answer) =>
            GetRoundValidationsForThemeAnswer(players, roundId, theme, answer).Count(x => x.Valid);

        public static int GetRoundInvalidVotesCountForThemeAnswer(this IEnumerable<Player> players, Guid roundId, string theme, string answer) =>
            GetRoundValidationsForThemeAnswer(players, roundId, theme, answer).Count(x => !x.Valid);

        private static IEnumerable<AnswerValidation> GetRoundValidationsForThemeAnswer(this IEnumerable<Player> players, Guid roundId, string theme, string answer)
        {
            var answerValidations = players
                .SelectMany(p => p.RoundsValidations
                    .Where(rv => rv.RoundId == roundId)
                    .SelectMany(rv => rv.Validations.Where(v => v.Theme.Equals(theme)))
                    .SelectMany(v => v.AnswersValidations.Where(av => av.Answer.Equals(answer))));

            return answerValidations;
        }

        public static IEnumerable<Player> GetPlayersThatRepliedAnswerForThemeInRound(this IEnumerable<Player> players, Guid roundId, string theme, string answer)
        {
            return players
                .Where(p => p.RoundsAnswers
                    .Where(ra => ra.RoundId == roundId)
                    .Any(a => a.Answers
                        .Any(ta => ta.Theme.Equals(theme) && ta.Answer.Equals(answer))));
        }

        public static IEnumerable<Player> GetPlayersThatNotRepliedAnswerForThemeInRound(this IEnumerable<Player> players, Guid roundId, string theme)
        {
            return players
                .Where(p => p.RoundsAnswers
                    .Where(ra => ra.RoundId == roundId)
                    .Any(a => !a.Answers.Any(ta => ta.Theme.Equals(theme)) || a.Answers.Any(ta => string.IsNullOrEmpty(ta.Answer))));
        }
    }
}
