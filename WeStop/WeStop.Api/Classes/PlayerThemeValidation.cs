using System.Collections.Generic;

namespace WeStop.Api.Classes
{
    public class PlayerThemeValidation
    {
        public PlayerThemeValidation()
        {
            Validations = new Dictionary<string, bool>();
        }

        public string Theme { get; set; }
        public IDictionary<string, bool> Validations { get; set; }
    }
}