using System;

namespace WeStop.Core.Exceptions
{
    public class WeStopException : Exception
    {
        public WeStopException(string message) : base(message)
        {
        }
    }
}