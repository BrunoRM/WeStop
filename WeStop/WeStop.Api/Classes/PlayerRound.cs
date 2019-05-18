using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public class PlayerRound
    {
        public PlayerRound()
        {
            ThemesAnswersValidations = new List<ThemeValidation>();
            ThemesPontuations = new Dictionary<string, int>();
            Answers = new List<ThemeAnswer>();
        }

        public Player Player { get; set; }
        public ICollection<ThemeAnswer> Answers { get; set; }
        public IDictionary<string, int> ThemesPontuations { get; set; }
        public ICollection<ThemeValidation> ThemesAnswersValidations { get; set; }
        public int EarnedPoints => ThemesPontuations.Values.Sum();

        public void AddThemeAnswersValidations(ThemeValidation validation)
        {
            if (!ThemesAnswersValidations.Any(x => x.Theme == validation.Theme))
                ThemesAnswersValidations.Add(validation);
        }

        private void AddAnswer(ThemeAnswer themeAnswer)
        {
            if (string.IsNullOrEmpty(themeAnswer.Answer))
                return;

            if (Answers.Any(x => x.Theme == themeAnswer.Theme))
                return;

            Answers.Add(themeAnswer);
        }

        public void AddAnswers(ICollection<ThemeAnswer> answers)
        {
            foreach (var answer in answers)
                AddAnswer(answer);
        }

        public void GeneratePointsForTheme(string theme, int points)
        {
            ThemesPontuations.Add(theme, points);
            Player.AddPoints(points);
        }
    }
}