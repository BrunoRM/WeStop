using MediatR;
using WeStop.Application.Dtos.Player;

namespace WeStop.Application.Commands.RegisterPlayer
{
    public class RegisterPlayerCommand : IRequest<PlayerDto>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
