using System;

namespace WeStop.Api.Classes
{
    public sealed class ThemeValidationTimerContext : GameTimerContext
    {
        public ThemeValidationTimerContext(Guid gameId, string themeBeingValidated, int limitTime) : base(gameId, limitTime)
        {
            ThemeBeingValidated = themeBeingValidated;
        }

        public string ThemeBeingValidated { get; private set; }
    }
}
