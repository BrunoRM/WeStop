using WeStop.Application.Errors;

namespace WeStop.Application.Exceptions
{
    public class NotFoundException
    {
        public string Error { get; private set; }

        public NotFoundException(string errorMessage)
        {
            Error = errorMessage;
        }
    }
}