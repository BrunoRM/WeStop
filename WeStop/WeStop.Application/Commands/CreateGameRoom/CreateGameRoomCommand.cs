using MediatR;
using WeStop.Application.Dtos.GameRoom;

namespace WeStop.Application.Commands.RegisterPlayer
{
    public class CreateGameRoomCommand : IRequest<GameRoomDto>
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int NumberOfRounds { get; set; }
        public int NumberOfPlayers { get; set; }
        public string[] Themes { get; set; }
        public string[] AvailableLetters { get; set; }
    }
}
