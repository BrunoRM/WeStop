using System;

namespace WeStop.Domain.Exceptions
{
    public class WeStopException : Exception
    {
        public WeStopException(string error)
        {
            Error = error;
        }

        public string Error { get; private set; }
    }
}
