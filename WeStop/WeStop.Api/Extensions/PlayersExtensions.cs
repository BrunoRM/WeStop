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
                    .Where(ra => ra.RoundNumber == roundId)
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
                .SelectMany(p => p.RoundsValidations.FirstOrDefault(rv => rv.RoundNumber == roundId)?.Validations
                    .Where(v => v.Theme.Equals(theme))
                    .SelectMany(v => v.AnswersValidations.Where(av => av.Answer.Equals(answer))));

            return answerValidations;
        }

        public static IEnumerable<Player> GetPlayersThatRepliedAnswerForThemeInRound(this IEnumerable<Player> players, Guid roundId, string theme, string answer)
        {
            return players
                .Where(p => p.RoundsAnswers.FirstOrDefault(ra => ra.RoundNumber == roundId)
                    .Answers.Any(a => a.Theme.Equals(theme) && a.Answer.Trim().Equals(answer)));
        }

        public static IEnumerable<Player> GetPlayersThatNotRepliedAnswerForThemeInRound(this IEnumerable<Player> players, Guid roundId, string theme)
        {
            return players
                .Where(p => p.RoundsAnswers.FirstOrDefault(ra => ra.RoundNumber == roundId)
                    .Answers.Any(a => !a.Theme.Equals(theme) || a.Theme.Equals(theme) && string.IsNullOrEmpty(a.Answer)));
        }
    
        public static ICollection<ThemeAnswers> GetPlayersAnswersInRoundExcept(this IEnumerable<Player> players, Guid roundId, Guid playerId)
        {
            ICollection<ThemeAnswer> answersOfOthersPlayers = GetAnswersOfOtherPlayersInRound(players, roundId, playerId);

            ICollection<ThemeAnswers> themesAnswers = new List<ThemeAnswers>();
            foreach (var themeAnswer in answersOfOthersPlayers)
            {
                string theme = themeAnswer.Theme;
                string answer = themeAnswer.Answer;

                ThemeAnswers existingThemeAnswers = themesAnswers.FirstOrDefault(ta => ta.Theme == theme);
                if (existingThemeAnswers != null && !existingThemeAnswers.HasAnswer(answer))
                    existingThemeAnswers.AddAnswer(answer);
                else
                {
                    ThemeAnswers themeAnswers = new ThemeAnswers(themeAnswer.Theme, themeAnswer.Answer);
                    themesAnswers.Add(themeAnswers);
                }
            }

            return themesAnswers;
        }

        private static ICollection<ThemeAnswer> GetAnswersOfOtherPlayersInRound(IEnumerable<Player> players, Guid roundId, Guid playerId)
        {
            return players.Where(p => p.Id != playerId)
                .SelectMany(p => p.RoundsAnswers.FirstOrDefault(ra => ra.RoundNumber == roundId).Answers).ToList();
        }

        public static ThemeAnswers GetCurrentRoundPlayersAnswersForThemeExceptFromPlayer(IEnumerable<Player> players, Guid roundId, Guid playerId, string theme)
        {
            ICollection<ThemeAnswer> answersOfOthersPlayers = GetAnswersForThemeOfOtherPlayersInRound(players, roundId, playerId, theme);

            ThemeAnswers themeAnswers = new ThemeAnswers(theme);
            foreach (var themeAnswer in answersOfOthersPlayers)
            {
                string answer = themeAnswer.Answer;

                if (!themeAnswers.HasAnswer(answer))
                    themeAnswers.AddAnswer(answer);
            }

            return themeAnswers;
        }

        private static ICollection<ThemeAnswer> GetAnswersForThemeOfOtherPlayersInRound(IEnumerable<Player> players, Guid roundId, Guid playerId, string theme)
        {
            return players.Where(p => p.Id != playerId)
                .SelectMany(p => p.RoundsAnswers.FirstOrDefault(ra => ra.RoundNumber == roundId).Answers
                    .Where(a => !string.IsNullOrEmpty(a.Answer) && a.Theme.Equals(theme)))
                    .ToList();
        }
    }
}
