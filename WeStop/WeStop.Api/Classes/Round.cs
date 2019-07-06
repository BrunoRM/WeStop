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
            //CurrentRoundTime = 0;
            //CurrentValidationTime = 0;
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

        public ICollection<PlayerRound> GetOnlinePlayers() =>
            Players.Where(pr => pr.Player.Status == PlayerStatus.Online).ToList();

        // TODO: Refatorar esse método (deixar mais legível)
        public ICollection<ThemeAnswers> GetPlayersAnswersExceptFromPlayer(Guid playerId)
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

        public bool IsFinished() =>
            Finished;
    }
}