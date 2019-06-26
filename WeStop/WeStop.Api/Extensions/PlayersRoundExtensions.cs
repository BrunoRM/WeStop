using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;

namespace WeStop.Api.Extensions
{
    public static class PlayersRoundExtensions
    {
        public static ICollection<AnswerValidation> GetValidationsForThemeAnswer(this ICollection<PlayerRound> playersInRound, string theme, string answer) =>
            playersInRound.SelectMany(playerRound => playerRound.ThemesAnswersValidations.Where(ta => ta.Theme == theme)).SelectMany(ta => ta.AnswersValidations.Where(av => av.Answer.Equals(answer))).ToList();

        public static int GetValidVotesCountForThemeAnswer(this ICollection<PlayerRound> playersInRound, string theme, string answer) =>
            GetValidationsForThemeAnswer(playersInRound, theme, answer).Count(x => x.Valid);

        public static int GetInvalidVotesCountForThemeAnswer(this ICollection<PlayerRound> playersInRound, string theme, string answer) =>
            GetValidationsForThemeAnswer(playersInRound, theme, answer).Count(x => !x.Valid);
    }
}
