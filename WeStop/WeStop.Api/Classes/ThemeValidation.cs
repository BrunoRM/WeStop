using System.Collections.Generic;

namespace WeStop.Api.Classes
{
    public class ThemeValidation
    {
        public ThemeValidation(string theme, ICollection<AnswerValidation> answersValidations)
        {
            Theme = theme;
            AnswersValidations = answersValidations;
        }

        public string Theme { get; set; }
        public ICollection<AnswerValidation> AnswersValidations { get; set; }
    }
}