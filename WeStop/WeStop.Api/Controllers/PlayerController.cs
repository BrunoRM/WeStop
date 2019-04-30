using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeStop.Application.Commands;

namespace WeStop.Api.Controllers
{
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlayerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("api/players.create"), HttpPost]
        public async Task<IActionResult> Register(RegisterPlayerRequest request) =>
            Ok(await _mediator.Send(request));
    }
}