using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Exceptions;

namespace WeStop.Api.Classes
{
    public class Round
    {
        public Round()
        {
            Finished = false;
            Players = new List<PlayerRound>();
            CurrentRoundTime = 0;
            CurrentValidationTime = 0;
        }

        public int Number { get; set; }
        public string SortedLetter { get; set; }
        public bool Finished { get; set; }
        public ICollection<PlayerRound> Players { get; set; }
        public int CurrentRoundTime { get; private set; }
        public int CurrentValidationTime { get; private set; }

        public void RefreshValidationTime() =>
            CurrentValidationTime++;

        public void RefreshRoundTime() =>
            CurrentRoundTime++;

        public ICollection<PlayerRound> GetPlayersOnline() =>
            Players.Where(pr => pr.Player.Status == PlayerStatus.Online).ToList();

        public ICollection<ThemeAnswers> GetPlayersAnswers(Guid playerId)
        {
            var themesAnswers = new List<ThemeAnswers>();

            var answers = Players.Where(p => p.Player.Id != playerId)
                .SelectMany(x => x.Answers);

            foreach (var answer in answers)
            {
                if (themesAnswers.Any(ta => ta.Theme == answer.Theme))
                {
                    if (!themesAnswers.Any(ta => ta.Theme == answer.Theme && ta.Answers.Any(a => a == answer.Answer)))
                    {
                        var themeAnswers = themesAnswers.First(ta => ta.Theme == answer.Theme);
                        themeAnswers.AddAnswer(answer.Answer);
                    }
                }
                else
                {
                    var themeAnswers = new ThemeAnswers(answer.Theme);
                    themeAnswers.AddAnswer(answer.Answer);
                    themesAnswers.Add(themeAnswers);
                }
            }

            return themesAnswers;
        }
    }

    public class ThemeAnswers
    {
        private ICollection<string> _answers;

        public ThemeAnswers(string theme)
        {
            Theme = theme;
            _answers = new List<string>();
        }

        public string Theme { get; private set; }
        public IReadOnlyCollection<string> Answers => _answers.ToList();

        public void AddAnswer(string answer) =>
            _answers.Add(answer);
    }
}