using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Dtos;
using WeStop.Api.Infra.Timers;
using WeStop.Core;
using WeStop.Core.Services;

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
            System.Console.WriteLine($"{Context.ConnectionId} saiu");
            var removedData = ConnectionBinding.RemoveConnectionIdBinding(Context.ConnectionId);
            if (removedData.playerId != null)
            {
                System.Console.WriteLine("Encontrou e removeu o player");
                var gameId = removedData.gameId;
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
            }

            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("join")]
        public async Task JoinAsync(Guid gameId, string password, User user)
        {
            await _gameManager.JoinAsync(gameId, password, user, async (game, player) =>
            {
                ConnectionBinding.BindConnectionId(Context.ConnectionId, user.Id, gameId);
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

                switch (game.State)
                {
                    case GameState.Waiting:

                        await Clients.Caller.SendAsync("im_joined_game", new
                        {
                            game = _mapper.Map<Game, GameDto>(game),
                            lastRoundScoreboard = game.GetScoreboard(game.PreviousRoundNumber),
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
                            (ICollection<Validation> Validations, int TotalValidations, int ValidationNumber) = await _gameManager.GetPlayerDefaultValidationsAsync(game.Id, game.CurrentRound.Number, player.Id, game.CurrentRound.ThemeBeingValidated, game.CurrentRound.SortedLetter);

                            await Clients.Caller.SendAsync("im_reconected_game", new
                            {
                                game = _mapper.Map<Game, GameDto>(game),
                                round = game.CurrentRound,
                                player = _mapper.Map<Player, PlayerDto>(player),
                                theme = game.CurrentRound.ThemeBeingValidated,
                                validations = Validations,
                                totalValidations = TotalValidations,
                                validationsNumber = ValidationNumber
                            });
                        }

                        break;

                    case GameState.Finished:

                        var winners = game.GetWinners().ToList();

                        await Clients.Caller.SendAsync("im_reconected_game", new
                        {
                            game = _mapper.Map<Game, GameDto>(game),
                            player = _mapper.Map<Player, PlayerDto>(player),
                            lastRoundScoreboard = game.GetScoreboard(game.PreviousRoundNumber),
                            winners
                        });

                        break;
                }
            }, async (error) => await Clients.Caller.SendAsync("game_join_error", error));
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
                await Clients.Group(gameId.ToString()).SendAsync("round_stoped", new
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
            await AddValidationsAndNotifyClientsAsync(roundValidations);

            var gameId = roundValidations.GameId;
            var theme = roundValidations.Theme;
            if (await _gameManager.CheckAllPlayersSendValidationsAsync(gameId, roundValidations.RoundNumber, theme))
            {
                await Clients.Group(gameId.ToString()).SendAsync("all_validations_sended", theme);
                await _gameManager.FinishValidationsForThemeAsync(gameId, theme);
                await _gameTimer.StartValidationForNextThemeAsync(gameId);
            }
        }

        [HubMethodName("send_validations_after_time_over")]
        public async Task SendValidationsAfterTimeOverAsync(RoundValidations roundValidations)
        {
            await AddValidationsAndNotifyClientsAsync(roundValidations);
        }

        private async Task AddValidationsAndNotifyClientsAsync(RoundValidations roundValidations)
        {
            await _gameManager.AddRoundValidationsAsync(roundValidations);
            await Clients.Caller.SendAsync("im_send_validations");
        }

        [HubMethodName("player_change_status")]
        public async Task ChangePlayerStatusAsync(Guid gameId, Guid playerId, bool isReady)
        {
            var player = await _playerManager.ChangeStatusAsync(gameId, playerId, isReady);

            await Clients.GroupExcept(gameId.ToString(), Context.ConnectionId).SendAsync("player_changed_status", new
            {
                player.Id,
                player.UserName,
                player.IsAdmin,
                player.IsReady
            });

            await Clients.Caller.SendAsync("im_change_status", new
            {
                isReady
            });
        }

        [HubMethodName("leave")]
        public async Task LeaveAsync(Guid gameId, Guid playerId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
            await _gameManager.LeaveAsync(gameId, playerId, async (isGameFinished, newAdmin) =>
            {
                if (!isGameFinished)
                {
                    await Clients.Group(gameId.ToString()).SendAsync("player_left", playerId);
                    if (newAdmin != null)
                    {
                        await Clients.Group(gameId.ToString()).SendAsync("new_admin_setted", newAdmin.Id);
                    }
                }
            });
        }
    }
}