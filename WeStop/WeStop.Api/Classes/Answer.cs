namespace WeStop.Api.Classes
{
    public struct Answer
    {
        public Answer(string theme, string answer)
        {
            Theme = theme;
            Value = answer?.Trim() ?? string.Empty;
        }

        public string Theme { get; private set; }
        public string Value { get; private set; }
    }
}