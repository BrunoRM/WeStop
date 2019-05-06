using System;
using WeStop.Domain.Exceptions;

namespace WeStop.Application.Exceptions
{

    public class ErrorException : WeStopException
    {
        public ErrorException(string error) : base(error)
        {
        }
    }
}