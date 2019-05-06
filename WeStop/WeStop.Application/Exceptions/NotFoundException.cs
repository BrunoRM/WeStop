using WeStop.Domain.Exceptions;

namespace WeStop.Application.Exceptions
{
    public class NotFoundException : WeStopException
    {
        public NotFoundException(string error) : base(error)
        {
        }
    }
}