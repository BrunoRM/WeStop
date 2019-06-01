using System;
using System.Runtime.Serialization;

namespace WeStop.Api.Exceptions
{
    public class WeStopException : Exception
    {
        public WeStopException(string message) : base(message)
        {
        }
    }
}