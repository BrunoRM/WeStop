namespace WeStop.Api.Classes
{
    public class ThemeAnswer
    {
        public ThemeAnswer(string theme, string answer)
        {
            Theme = theme;
            Answer = answer.Trim();
        }

        public string Theme { get; private set; }
        public string Answer { get; private set; }
    }
}