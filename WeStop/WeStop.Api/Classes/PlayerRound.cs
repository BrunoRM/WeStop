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
        }

        public Player Player { get; set; }
        public ICollection<ThemeAnswer> Answers { get; set; }
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

        public void AddValidatiosForTheme(string theme, ICollection<AnswerValidation> validations)
        {
            bool hasValidationForThemeAnswer = ThemesAnswersValidations.Any(ta => ta.Theme == theme);

            if (!hasValidationForThemeAnswer)
            {
                ThemeValidation themeValidation = new ThemeValidation(theme, validations);
                _themesValidations.Add(themeValidation);
            }
        }

        public void AddAnswerForTheme(string theme, string answer)
        {
            bool hasAnswerForTheme = Answers.Any(a => a.Theme.Equals(theme));

            if (!hasAnswerForTheme)
            {
                var themeAnswer = new ThemeAnswer(theme, answer);
                Answers.Add(themeAnswer);
            }
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