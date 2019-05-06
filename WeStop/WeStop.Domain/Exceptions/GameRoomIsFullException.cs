using WeStop.Domain.Errors;

namespace WeStop.Domain.Exceptions
{
    public class GameRoomIsFullException : WeStopException
    {
        public GameRoomIsFullException() 
            : base(GameRoomErrors.IsFull)
        {
        }
    }
}
