using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
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
