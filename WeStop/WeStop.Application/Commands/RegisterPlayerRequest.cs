using MediatR;
using WeStop.Application.Dtos.Player;
using WeStop.Common.Handlers;

namespace WeStop.Application.Commands
{
    public class RegisterPlayerRequest : IRequest<PlayerDto>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
