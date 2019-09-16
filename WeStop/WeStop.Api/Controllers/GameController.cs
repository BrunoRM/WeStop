using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeStop.Api.Domain;
using WeStop.Api.Dtos;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.Api.Managers;

namespace WeStop.Api.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameStorage _gameStorage;
        private readonly GameManager _gameManager;

        public GameController(IGameStorage gameStorage, GameManager gameManager)
        {
            _gameStorage = gameStorage;
            _gameManager = gameManager;
        }

        [Route("api/games.create"), HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody]CreateGameDto dto)
        {
            var createdGame = await _gameManager.CreateAsync(dto.UserId, dto.Name, "", dto.GameOptions);

            return Ok(new
            {
                ok = true,
                id = createdGame.Id
            });
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