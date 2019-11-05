using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Dtos;
using WeStop.Api.Infra.Timers;
using WeStop.Core;
using WeStop.Core.Helpers;
using WeStop.Core.Services;
using WeStop.Core.Storages;

namespace WeStop.Api.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameStorage _gameStorage;
        private readonly GameManager _gameManager;
        private readonly GameTimer _gamesTimers;
        private readonly IMapper _mapper;

        public GameController(IGameStorage gameStorage, GameManager gameManager,
            GameTimer gamesTimers, IMapper mapper)
        {
            _gameStorage = gameStorage;
            _gameManager = gameManager;
            _gamesTimers = gamesTimers;
            _mapper = mapper;
        }

        [Route("api/games.create"), HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody]CreateGameDto dto)
        {
            var createdGame = await _gameManager.CreateAsync(dto.User, dto.Name, dto.Password, dto.GameOptions);

            _gamesTimers.Register(createdGame.Id, createdGame.Options.RoundTime);

            return Ok(new
            {
                id = createdGame.Id
            });
        }
        
        [Route("api/games.list"), HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var games = await _gameStorage.GetAsync();

            return Ok(new
            {
                games = games.Select(g => 
                    _mapper.Map<Game, GameSummary>(g))
            });
        }

        [Route("api/games.authorize")]
        public async Task<IActionResult> CheckAsync([FromQuery]Guid gameId, [FromQuery]string password, [FromBody] User user)
        {
            var status = await _gameManager.AuthorizePlayerAsync(gameId, password, user);

            if (status.Equals("OK"))
            {
                return Ok(new
                {
                    ok = true,
                    gameId
                });
            }
            else
            {
                return Ok(new
                {
                    ok = false,
                    error = status
                });
            }
        }
    }
}