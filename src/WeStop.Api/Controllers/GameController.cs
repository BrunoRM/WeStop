using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Dtos;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.Api.Infra.Timers;
using WeStop.Api.Managers;

namespace WeStop.Api.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameStorage _gameStorage;
        private readonly GameManager _gameManager;
        private readonly GameTimer _gamesTimers;

        public GameController(IGameStorage gameStorage, GameManager gameManager,
            GameTimer gamesTimers)
        {
            _gameStorage = gameStorage;
            _gameManager = gameManager;
            _gamesTimers = gamesTimers;
        }

        [Route("api/games.create"), HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody]CreateGameDto dto)
        {
            var createdGame = await _gameManager.CreateAsync(dto.UserId, dto.Name, "", dto.GameOptions);

            _gamesTimers.Register(createdGame.Id, createdGame.Options.RoundTime);

            return Ok(new
            {
                id = createdGame.Id
            });
        }
        
        [Route("api/games.list"), HttpGet]
        public async Task<IActionResult> GetThemesAsync()
        {
            var games = await _gameStorage.GetAsync();

            return Ok(new
            {
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
                    currentRound = g.CurrentRound?.Number ?? 1
                })
            });
        }
    }
}