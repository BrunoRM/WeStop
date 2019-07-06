using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public class PlayerRound
    {
        private ICollection<ThemeValidation> _themesValidations;
        public PlayerRound()
        {
            _themesValidations = new List<ThemeValidation>();
            ThemesPontuations = new Dictionary<string, int>();
            Answers = new List<ThemeAnswer>();
            AnswersSended = false;
        }

        public Player Player { get; set; }
        public ICollection<ThemeAnswer> Answers { get; set; }
        public bool AnswersSended { get; private set; }
        public IDictionary<string, int> ThemesPontuations { get; set; }
        public IReadOnlyCollection<ThemeValidation> ThemesAnswersValidations => _themesValidations.ToList();
        public int EarnedPoints => ThemesPontuations.Values.Sum();

        public void AddThemeAnswersValidations(ThemeValidation validation)
        {
            bool hasValidationForThemeAnswer = ThemesAnswersValidations.Any(ta => ta.Theme == validation.Theme);

            if (!hasValidationForThemeAnswer)
            {
                _themesValidations.Add(validation);
            }
            else
            {
                ThemeValidation themeValidation = ThemesAnswersValidations.First(tv => tv.Theme == validation.Theme);

                foreach (AnswerValidation answerValidation in themeValidation.AnswersValidations)
                {
                    if (!validation.AnswersValidations.Any(av => av.Answer == answerValidation.Answer))
                    {
                        continue;
                    }
                    else
                    {
                        answerValidation.Valid = validation.AnswersValidations.First(av => av.Answer == answerValidation.Answer).Valid;
                    }
                }
            }
        }

        public void AddAnswers(ICollection<ThemeAnswer> answers)
        {
            foreach (var answer in answers)
            {
                bool alreadyHasAnswerForTheme = Answers.Any(a => a.Theme.Equals(answer.Theme));

                if (alreadyHasAnswerForTheme)
                    return;

                Answers.Add(answer);
            }

            AnswersSended = true;
        }

        public void GeneratePointsForTheme(string theme, int points)
        {
            ThemesPontuations.Add(theme, points);
            Player.AddPoints(points);
        }

        public bool HasValidationForTheme(string theme) =>
            ThemesAnswersValidations.Any(themeValidation => themeValidation.Theme == theme);

        public ICollection<ThemeValidation> GetThemeValidations()
        {
            return ThemesAnswersValidations.ToList();
        }
    }
}