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

        [Route("api/games.check")]
        public async Task<IActionResult> CheckAsync([FromQuery]Guid gameId, [FromQuery]string password)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);

            if (game.IsPrivate())
            {
                if (string.IsNullOrEmpty(password))
                {
                    return Ok(new
                    {
                        ok = false,
                        error = "PASSWORD_REQUIRED"
                    });
                }
                else
                {
                    var providedPasswordHash = MD5HashGenerator.GenerateHash(password);
                    if (!game.Password.Equals(providedPasswordHash))
                    {
                        return Ok(new
                        {
                            ok = false,
                            error = "PASSWORD_INCORRECT"
                        });
                    }
                }
            }

            if (game.IsFull())
            {
                return Ok(new
                {
                    ok = false,
                    error = "GAME_FULL"
                });
            }

            if (game.IsFinalRound())
            {
                return Ok(new
                {
                    ok = false,
                    error = "GAME_IN_FINAL_ROUND"
                });
            }

            return Ok(new
            {
                ok = true,
                gameId
            });
        }
    }
}