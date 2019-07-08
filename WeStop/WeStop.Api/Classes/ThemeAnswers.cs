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

        public ThemeAnswers(string theme, string answer)
            : this(theme)
        {
            AddAnswer(answer);
        }

        public string Theme { get; private set; }
        public IReadOnlyCollection<string> Answers => _answers.ToList();

        public void AddAnswer(string answer)
        {
            if (!Answers.Contains(answer))
            {
                _answers.Add(answer);
            }
        }

        public bool HasAnswer(string answer) =>
            Answers.Any(a => a.Equals(answer));
    }
}
