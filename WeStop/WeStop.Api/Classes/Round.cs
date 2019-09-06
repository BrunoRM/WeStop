using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public class Round
    {
        private Round()
        {
            Finished = false;
            Players = new List<PlayerRound>();
        }

        public Round(int number, string sortedLetter, ICollection<PlayerRound> players)
            : this()
        {
            Number = number;
            SortedLetter = sortedLetter;
            Players = players;
        }

        public Guid Id { get; set; }
        public int Number { get; private set; }
        public string SortedLetter { get; private set; }
        public bool Finished { get; private set; }
        public ICollection<PlayerRound> Players { get; set; }

        public ICollection<PlayerRound> GetPlayers() =>
            Players.ToList();

        public ICollection<ThemeAnswers> GetPlayersAnswersExceptFromPlayer(Guid playerId)
        {
            ICollection<ThemeAnswer> answersOfOthersPlayers = GetAnswersOfOtherPlayers(playerId);

            ICollection<ThemeAnswers> themesAnswers = new List<ThemeAnswers>();
            foreach (var themeAnswer in answersOfOthersPlayers)
            {
                string theme = themeAnswer.Theme;
                string answer = themeAnswer.Answer;

                ThemeAnswers existingThemeAnswers = themesAnswers.FirstOrDefault(ta => ta.Theme == theme);
                if (existingThemeAnswers != null && !existingThemeAnswers.HasAnswer(answer))
                {
                    existingThemeAnswers.AddAnswer(answer);
                }
                else
                {
                    ThemeAnswers themeAnswers = new ThemeAnswers(themeAnswer.Theme, themeAnswer.Answer);
                    themesAnswers.Add(themeAnswers);
                }
            }

            return themesAnswers;
        }

        public ThemeAnswers GetCurrentRoundPlayersAnswersForThemeExceptFromPlayer(string theme, Guid playerId)
        {
            ICollection<ThemeAnswer> answersOfOthersPlayers = GetThemeAnswersOfOtherPlayers(theme, playerId);

            ThemeAnswers themeAnswers = new ThemeAnswers(theme);
            foreach (var themeAnswer in answersOfOthersPlayers)
            {
                string answer = themeAnswer.Answer;

                if (!themeAnswers.HasAnswer(answer))
                {
                    themeAnswers.AddAnswer(answer);
                }
            }

            return themeAnswers;
        }

        private ICollection<ThemeAnswer> GetAnswersOfOtherPlayers(Guid playerId)
        {
            return Players.Where(p => p.Player.Id != playerId)
                .SelectMany(x => x.Answers).ToList();
        }

        private ICollection<ThemeAnswer> GetThemeAnswersOfOtherPlayers(string theme, Guid playerId)
        {
            return Players.Where(p => p.Player.Id != playerId)
                .SelectMany(x => x.Answers.Where(a => a.Theme == theme)).ToList();
        }
    }
}