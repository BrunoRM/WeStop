using System;
using WeStop.Application.Errors;

namespace WeStop.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public string Error { get; private set; }

        public NotFoundException()
        {
            Error = CommonErrors.NotFound;
        }
    }
}