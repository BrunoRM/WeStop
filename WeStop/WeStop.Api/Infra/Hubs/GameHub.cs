using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Dtos;
using WeStop.Api.Extensions;
using WeStop.Api.Infra.Timers;
using WeStop.Api.Managers;

namespace WeStop.Api.Infra.Hubs
{
    public class GameHub : Hub
    {
        private readonly GameManager _gameManager;
        private readonly PlayerManager _playerManager;
        private readonly GameTimer _gameTimer;
        private readonly RoundScorerManager _roundScorer;
        private readonly IMapper _mapper;

        public GameHub(GameTimer gameTimer, RoundScorerManager roundScorer, IMapper mapper, 
            GameManager gameManager, PlayerManager playerManager)
        {
            _gameManager = gameManager;
            _playerManager = playerManager;
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
            await _gameManager.JoinAsync(gameId, userId, async (game, player) =>
            {
                ConnectionBinding.BindConnectionId(Context.ConnectionId, userId, game.Id);
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

                switch (game.State)
                {
                    case GameState.Waiting:

                        await Clients.Caller.SendAsync("im_joined_game", new
                        {
                            game = _mapper.Map<Game, GameDto>(game),
                            player = _mapper.Map<Player, PlayerDto>(player)
                        });

                        await Clients.GroupExcept(game.Id.ToString(), Context.ConnectionId).SendAsync("player_joined_game", new
                        {
                            player = _mapper.Map<Player, PlayerDto>(player)
                        });

                        break;

                    case GameState.InProgress:

                        if (player.InRound)
                        {
                            await Clients.Caller.SendAsync("im_reconected_game", new
                            {
                                game = _mapper.Map<Game, GameDto>(game),
                                player = _mapper.Map<Player, PlayerDto>(player),
                                round = game.CurrentRound
                            });
                        }

                        break;

                    case GameState.Validations:

                        if (player.InRound)
                        {
                            var defaultPlayerValidations = await _gameManager.GetPlayerDefaultValidationsAsync(game.Id, game.CurrentRound.Number, player.Id);

                            if (defaultPlayerValidations.Any())
                            {
                                await Clients.Caller.SendAsync("im_reconected_game", new
                                {
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
            await _gameManager.StartRoundAsync(gameId, async (game) =>
            {
                var round = game.CurrentRound;

                IClientProxy connectionGroup = Clients.Group(gameId.ToString());
                await connectionGroup.SendAsync("round_started", new
                {
                    roundNumber = round.Number,
                    sortedLetter = round.SortedLetter
                });

                int limitTime = game.Options.Time;
                _gameTimer.StartRoundTimer(game.Id, limitTime, async (id, currentTime, hub) =>
                {
                    await hub.Group(id).SendAsync("round_time_elapsed", currentTime);
                },
                async (id, hub) =>
                {
                    StopRoundTimer(game.Id, round.Number);
                    await hub.Group(id).SendAsync("round_stop", new
                    {
                        reason = "time_over"
                    });
                });
            });
        }

        [HubMethodName("round_stop")]
        public async Task StopRoundAsync(Guid gameId, Guid playerId)
        {
            await _gameManager.StopCurrentRoundAsync(gameId, async (game) =>
            {
                await Clients.Group(gameId).SendAsync("round_stop", new
                {
                    reason = "player_call_stop",
                    playerId
                });

                StopRoundTimer(game.Id, game.CurrentRound.Number);
            });
        }

        [HubMethodName("send_answers")]
        public async Task SendAnswersAsync(Guid gameId, Guid playerId, ICollection<Answer> answers)
        {
            await _gameManager.AddCurrentRoundAnswersAsync(gameId, playerId, answers);
            await Clients.Caller.SendAsync("im_send_answers");
        }

        [HubMethodName("send_validations")]
        public async Task SendValidationsAsync(Guid gameId, Guid playerId, ICollection<Validation> validations)
        {
            await _gameManager.AddCurrentRoundValidationsAsync(gameId, playerId, validations);
            await Clients.Caller.SendAsync("im_send_validations");
        }

        [HubMethodName("player_change_status")]
        public async Task ChangePlayerStatusAsync(Guid gameId, Guid playerId, bool isReady)
        {
            await _playerManager.ChangeStatusAsync(gameId, playerId, isReady, async (player) =>
            {
                await Clients.GroupExcept(gameId.ToString(), Context.ConnectionId).SendAsync("player_changed_status", new
                {
                    player = new
                    {
                        player.Id,
                        player.UserName,
                        player.IsAdmin,
                        player.IsReady
                    }
                });

                await Clients.Caller.SendAsync("im_change_status");
            });
        }

        private void StopRoundTimer(Guid gameId, int roundNumber)
        {
            _gameTimer.StopRoundTimer(gameId);
            _gameTimer.StartSendAnswersTimer(gameId, (id, elapsedTime, hub) => { }, async (id, hub) =>
            {
                await hub.Group(gameId).SendAsync("send_answers_time_over");

                var gameConnectionsIds = ConnectionBinding.GetGameConnections(gameId);

                var playersValidations = await _gameManager.GetPlayersDefaultValidationsAsync(gameId, roundNumber);

                foreach (var (playerId, validations) in playersValidations)
                {
                    if (gameConnectionsIds.Any(gc => gc.PlayerId == playerId))
                    {
                        string connectionId = gameConnectionsIds.First(gc => gc.PlayerId == playerId).ConnectionId;
                        await hub.Clients.Client(connectionId).SendAsync("validation_started", validations);
                    }
                }

                _gameTimer.StartValidationTimer(gameId, async (gId, currentTime, hubContext) =>
                {
                    await hubContext.Group(gameId).SendAsync("validation_time_elapsed", new
                    {
                        currentTime
                    });
                }, async (gId, hubContext) =>
                {
                    await _gameManager.FinishCurrentRoundAsync(gId);
                    await _roundScorer.ProcessCurrentRoundPontuationAsync(gameId, async (roundPontuation) =>
                    {
                        await hubContext.Group(gameId).SendAsync("round_finished", new
                        {
                            scoreboard = roundPontuation
                        });
                    });
                });
            });
        }
    }
}