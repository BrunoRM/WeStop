using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameStorage _gameStorage;

        public GameController(IGameStorage gameStorage)
        {
            _gameStorage = gameStorage;
        }
        
        [Route("api/games.list"), HttpGet]
        public async Task<IActionResult> GetThemesAsync()
        {
            var games = await _gameStorage.GetAsync();

            return Ok(new
            {
                ok = true,
                games = games.Select(g => new 
                {
                    g.Id,
                    g.Name,
                    isPrivate = string.IsNullOrEmpty(g.Password) ? false : true,
                    g.Options.Themes,
                    g.Options.Rounds,
                    availableLetters = g.Options.AvailableLetters.Keys,
                    g.Options.NumberOfPlayers,
                    playersInGame = g.Players.Count,
                    currentRound = g.GetNextRoundNumber()
                })
            });
        }
    }
}