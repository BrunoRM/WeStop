using System;

namespace WeStop.Api.Domain
{
    public struct Answer : IEquatable<Answer>
    {
        public Answer(string theme, string value)
        {
            Theme = theme;
            Value = value?.Trim() ?? string.Empty;
        }

        public string Theme { get; private set; }
        public string Value { get; private set; }

        public bool Equals(Answer other) =>
            Theme == other.Theme && Value == other.Value;

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Theme.GetHashCode();
            hash = (hash * 7) + Value.GetHashCode();
            
            return hash;
        }

        public static bool operator ==(Answer a1, Answer a2) =>
            a1.Equals(a2);

        public static bool operator !=(Answer a1, Answer a2) =>
            !a1.Equals(a2);
    }
}