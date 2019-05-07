using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeStop.Application.Commands.RegisterPlayer;
using WeStop.Application.Queries.CheckGameRoomPlayer;
using WeStop.Application.Queries.GetWaitingGameRooms;

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
        public async Task<IActionResult> CreateNewGameRoomAsync(CreateGameRoomCommand request)
        {
            var gameRoom = await _mediator.Send(request);

            return Ok(new
            {
                ok = true,
                gameRoom
            });
        }

        [Route("api/gamerooms.list"), HttpGet]
        public async Task<IActionResult> GetGameRoomsInWaitingAsync([FromQuery]GetGameRoomsQuery request)
        {
            var gameRooms = await _mediator.Send(request);

            return Ok(new
            {
                ok = true,
                gameRooms
            });
        }

        [Route("api/gamerooms.players.check"), HttpGet]
        public async Task<IActionResult> CheckIfPlayerIsInGameAsync([FromBody]CheckGameRoomPlayerQuery request)
        {
            var playerIsInGame = await _mediator.Send(request);

            return Ok(new
            {
                ok = true,
                in_game = playerIsInGame
            });
        }

    }
}