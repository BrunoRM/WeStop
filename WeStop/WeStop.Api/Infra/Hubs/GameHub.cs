using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Dtos;
using WeStop.Api.Extensions;
using WeStop.Api.Helpers;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.Api.Infra.Timers.Interfaces;
using WeStop.Api.Managers;

namespace WeStop.Api.Infra.Hubs
{
    public class GameHub : Hub
    {
        private readonly GameManager _gameManager;
        private readonly IGameStorage _games;
        private readonly IAnswerStorage _answersStorage;
        private readonly IValidationStorage _validationStorage;
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IGameTimer _gameTimer;
        private readonly RoundScorer _roundScorer;
        private readonly IMapper _mapper;

        public GameHub(IGameStorage gameStorage, IAnswerStorage answersStorage,
            IValidationStorage validationStorage, IHubContext<GameHub> hubContext,
            IGameTimer gameTimer, RoundScorer roundScorer, IMapper mapper, GameManager gameManager)
        {
            _gameManager = gameManager;
            _games = gameStorage;
            _answersStorage = answersStorage;
            _validationStorage = validationStorage;
            _hubContext = hubContext;
            _gameTimer = gameTimer;
            _roundScorer = roundScorer;
            _mapper = mapper;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionBinding.RemoveConnectionIdBinding(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("join")]
        public async Task JoinAsync(Guid gameId, Guid userId)
        {
            await _gameManager.JoinToGameAsync(gameId, userId, async (game, player) =>
            {
                ConnectionBinding.BindConnectionId(Context.ConnectionId, userId, game.Id);
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

                GameState gameState = game.GetCurrentState();

                switch (gameState)
                {
                    case GameState.Waiting:

                        await Clients.Caller.SendAsync("im_joined_game", new
                        {
                            ok = true,
                            game = _mapper.Map<Game, GameDto>(game),
                            player = _mapper.Map<Player, PlayerDto>(game.GetPlayer(userId))
                        });

                        await Clients.GroupExcept(game.Id.ToString(), Context.ConnectionId).SendAsync("player_joined_game", new
                        {
                            ok = true,
                            player = _mapper.Map<Player, PlayerDto>(game.GetPlayer(userId))
                        });

                        break;

                    case GameState.InProgress:

                        if (player.IsInRound)
                        {
                            await Clients.Caller.SendAsync("im_reconected_game", new
                            {
                                ok = true,
                                game = _mapper.Map<Game, GameDto>(game),
                                player = _mapper.Map<Player, PlayerDto>(game.GetPlayer(userId)),
                                round = game.CurrentRound
                            });
                        }

                        break;

                    case GameState.ThemesValidations:

                        if (player.IsInRound)
                        {
                            var defaultPlayerValidations = await _gameManager.GetDefaultPlayerValidationsAsync(game.Id, game.CurrentRound.Number, player.Id);

                            if (defaultPlayerValidations.Any())
                            {
                                await Clients.Caller.SendAsync("im_reconected_game", new
                                {
                                    ok = true,
                                    game = _mapper.Map<Game, GameDto>(game),
                                    validations = defaultPlayerValidations
                                });
                            }
                        }

                        break;

                    case GameState.Finished:

                        Guid[] gameWinners = await _gameManager.GetWinnersAsync(game.Id);

                        await Clients.Caller.SendAsync("im_reconected_game", new
                        {
                            game = _mapper.Map<Game, GameDto>(game),
                            winners = gameWinners
                        });

                        break;
                }
            });            
        }

        [HubMethodName("start_round")]
        public async Task StartNextRoundAsync(Guid gameId)
        {
            await _gameManager.StartNextRoundAsync(gameId, async (game) =>
            {
                var round = game.CurrentRound;

                IClientProxy connectionGroup = Clients.Group(gameId.ToString());
                await connectionGroup.SendAsync("round_started", new
                {
                    ok = true,
                    roundNumber = round.Number,
                    sortedLetter = round.SortedLetter
                });

                int limitTime = game.Options.Time;
                _gameTimer.StartRoundTimer(game.Id, limitTime, async (id, currentTime) =>
                {
                    await _hubContext.Group(id).SendAsync("round_time_elapsed", currentTime);
                },
                async (id) =>
                {
                    StopRoundTimer(game.Id, round.Number);
                    await _hubContext.Group(id).SendAsync("round_stop", new
                    {
                        ok = true,
                        reason = "time_over"
                    });
                });
            });
        }

        [HubMethodName("round_stop")]
        public async Task StopRoundAsync(Guid gameId, Guid userId)
        {
            await _gameManager.StopCurrentRoundAsync(gameId, userId, async (game) =>
            {
                await Clients.Group(gameId).SendAsync("round_stop", new
                {
                    ok = true,
                    reason = "player_call_stop",
                    userId
                });

                StopRoundTimer(game.Id, game.CurrentRound.Number);
            });
        }

        [HubMethodName("send_answers")]
        public async Task SendAnswersAsync(RoundAnswers answers)
        {
            await _answersStorage.AddAsync(answers);

            await Clients.Caller.SendAsync("im_send_answers", new
            {
                ok = true
            });
        }

        [HubMethodName("send_validations")]
        public async Task SendValidationsAsync(RoundValidations validations)
        {
            await _validationStorage.AddAsync(validations);

            await Clients.Caller.SendAsync("im_send_validations", new
            {
                ok = true
            });
        }

        [HubMethodName("player_change_status")]
        public async Task ChangePlayerStatusAsync(ChangePlayerStatusDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameId);

            var player = game.Players.FirstOrDefault(x => x.User.Id == dto.UserId);
            player.ChangeStatus(dto.IsReady);

            await Clients.GroupExcept(dto.GameId.ToString(), Context.ConnectionId).SendAsync("player_status_changed", new
            {
                ok = true,
                player = new
                {
                    player.User.Id,
                    player.User.UserName,
                    player.IsAdmin,
                    player.IsReady
                }
            });

            await Clients.Caller.SendAsync("im_change_status", new
            {
                ok = true
            });
        }

        private void StopRoundTimer(Guid gameId, int roundNumber)
        {
            _gameTimer.StopRoundTimer(gameId);
            _gameTimer.StartSendAnswersTime(gameId, (id, elapsedTime) => { }, async (id) =>
            {
                await OnSendAnswersTimeOver(id, roundNumber);
            });
        }

        private async Task OnSendAnswersTimeOver(Guid gameId, int roundNumber)
        {
            await _hubContext.Group(gameId).SendAsync("send_answers_time_over");

            var gameConnectionsIds = ConnectionBinding.GetGameConnections(gameId);

            var roundAnswers = await _answersStorage.GetPlayersAnswersAsync(gameId, roundNumber);
            foreach (var connection in gameConnectionsIds)
            {
                var defaultPlayerValidations = roundAnswers.BuildValidationsForPlayer(connection.PlayerId).ToList();
                await _hubContext.Clients.Client(connection.ConnectionId).SendAsync("validation_started", defaultPlayerValidations);
            }

            _gameTimer.StartValidationTimer(gameId, async (id, currentTime) =>
            {
                await _hubContext.Group(gameId).SendAsync("validation_time_elapsed", new
                {
                    currentTime
                });
            }, async (id) =>
            {
                await _roundScorer.ProcessCurrentRoundPontuationAsync(gameId);
                await FinishCurrentRoundAsync(gameId);
            });
        }

        private async Task FinishCurrentRoundAsync(Guid gameId)
        {
            await _gameManager.FinishCurrentRoundAsync(gameId, async (roundPontuation) =>
            {
                await _hubContext.Group(gameId).SendAsync("round_finished", new
                {
                    ok = true,
                    scoreboard = roundPontuation
                });
            });
        }

        private async Task FinishGame(Game game)
        {
            var winners = await _gameManager.GetWinnersAsync(game.Id);

            game.Finish();
            await _hubContext.Group(game.Id).SendAsync("game_finished", new
            {
                ok = true,
                winners
            });
        }
    }
}