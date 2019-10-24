using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeStop.Core.Storages;

namespace WeStop.Api.Controllers
{
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly IThemeStorage _themeStorage;

        public ThemeController(IThemeStorage themeStorage)
        {
            _themeStorage = themeStorage;
        }

        [Route("api/themes.list"), HttpGet]
        public async Task<IActionResult> GetThemesAsync()
        {
            var themes = await _themeStorage.GetAsync();

            return Ok(new
            {
                themes
            });
        }
    }
}