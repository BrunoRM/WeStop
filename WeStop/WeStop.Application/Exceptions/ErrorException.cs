using System;

namespace WeStop.Application.Exceptions
{
    
    public class ErrorException : Exception
    {
        public string Error { get; private set; }
        
        public ErrorException(string errorMessage)
        {
            Error = errorMessage;
        }
    }
}