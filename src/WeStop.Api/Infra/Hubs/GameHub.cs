using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
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
        private readonly IMapper _mapper;

        public GameHub(GameTimer gameTimer, IMapper mapper, 
            GameManager gameManager, PlayerManager playerManager)
        {
            _gameManager = gameManager;
            _playerManager = playerManager;
            _gameTimer = gameTimer;
            _mapper = mapper;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (ConnectionBinding.RemoveConnectionIdBinding(Context.ConnectionId, out Guid? gameId, out Guid? playerId))
            {
                // The player don't necessarily are disconected, this will be changed for us let know
                // when player really left game or just refreshed.

                // await Clients.Group(gameId.ToString()).SendAsync("player_left", new
                // {
                //     id = playerId
                // });

                await base.OnDisconnectedAsync(exception);
            }
        }

        [HubMethodName("join")]
        public async Task JoinAsync(Guid gameId, User user)
        {
            await _gameManager.JoinAsync(gameId, user, async (game, player) =>
            {
                ConnectionBinding.BindConnectionId(Context.ConnectionId, user.Id, gameId);
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
                                    player = _mapper.Map<Player, PlayerDto>(player),
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
            await _gameManager.StartRoundAsync(gameId, async (createdRound) =>
            {
                IClientProxy connectionGroup = Clients.Group(gameId.ToString());
                await connectionGroup.SendAsync("round_started", new
                {
                    roundNumber = createdRound.Number,
                    sortedLetter = createdRound.SortedLetter
                });

                _gameTimer.StartRoundTimer(gameId);
            });
        }

        [HubMethodName("stop_round")]
        public async Task StopRoundAsync(Guid gameId, Guid playerId)
        {
            await _gameManager.StopCurrentRoundAsync(gameId, async (game) =>
            {
                await Clients.Group(gameId).SendAsync("round_stoped", new
                {
                    reason = "player_call_stop",
                    playerId
                });

                _gameTimer.StopRoundTimer(gameId);
            });
        }

        [HubMethodName("send_answers")]
        public async Task SendAnswersAsync(RoundAnswers roundAnswers)
        {
            await _gameManager.AddRoundAnswersAsync(roundAnswers);
            await Clients.Caller.SendAsync("im_send_answers");
        }

        [HubMethodName("send_validations")]
        public async Task SendValidationsAsync(RoundValidations roundValidations)
        {
            await _gameManager.AddRoundValidationsAsync(roundValidations);
            await Clients.Caller.SendAsync("im_send_validations");

            var gameId = roundValidations.GameId;
            if (await _gameManager.AllPlayersSendValidationsAsync(gameId))
            {
                await Clients.Group(gameId.ToString()).SendAsync("all_validations_sended");
            }
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
    }
}