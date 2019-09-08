namespace WeStop.Api.Classes
{
    public class ThemePontuation
    {
        public ThemePontuation(string theme, int pontuation)
        {
            this.Theme = theme;
            this.Pontuation = pontuation;

        }
        public string Theme { get; set; }
        public int Pontuation { get; set; }
    }
}
