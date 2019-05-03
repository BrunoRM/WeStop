using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeStop.Application.Commands.RegisterPlayer;

namespace WeStop.Api.Controllers
{
    [ApiController]
    public class GameRoomController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GameRoomController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("api/gamerooms.create"), HttpPost]
        public async Task<IActionResult> CreateNewGameRoom(CreateGameRoomCommand request)
        {
            var result = await _mediator.Send(request);

            return Ok(result);
        }
    }
}