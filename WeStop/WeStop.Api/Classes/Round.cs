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

        private ICollection<ThemeAnswer> GetAnswersOfOtherPlayers(Guid playerId)
        {
            return Players.Where(p => p.Player.Id != playerId)
                .SelectMany(x => x.Answers).ToList();
        }
    }
}