using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeStop.Application.Queries.GetThemes;

namespace WeStop.Api.Controllers
{
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ThemeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("api/themes.list"), HttpGet]
        public async Task<IActionResult> GetThemesAsync()
        {
            var themes = await _mediator.Send(new GetThemesQuery());

            return Ok(new
            {
                ok = true,
                themes
            });
        }
    }
}