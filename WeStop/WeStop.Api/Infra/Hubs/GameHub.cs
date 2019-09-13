using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Dtos;
using WeStop.Api.Extensions;
using WeStop.Api.Helpers;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.Api.Infra.Timers.Interfaces;

namespace WeStop.Api.Infra.Hubs
{
    public class GameHub : Hub
    {
        private readonly IUserStorage _users;
        private readonly IGameStorage _games;
        private readonly IAnswerStorage _answersStorage;
        private readonly IValidationStorage _validationStorage;
        private readonly IPontuationStorage _pontuationStorage;
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IGameTimer _gameTimer;
        private readonly RoundScorer _roundScorer;
        private readonly IMapper _mapper;

        public GameHub(IUserStorage userStorage, IGameStorage gameStorage, IAnswerStorage answersStorage,
            IValidationStorage validationStorage, IPontuationStorage pontuationStorage, IHubContext<GameHub> hubContext,
            IGameTimer gameTimer, RoundScorer roundScorer, IMapper mapper)
        {
            _users = userStorage;
            _games = gameStorage;
            _answersStorage = answersStorage;
            _validationStorage = validationStorage;
            _pontuationStorage = pontuationStorage;
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

        [HubMethodName("game_create")]
        public async Task CreateGame(CreateGameDto dto)
        {
            User user = await _users.GetByIdAsync(dto.UserId);

            Game game = new Game(user, dto.Name, string.Empty, new GameOptions(dto.GameOptions.Themes, dto.GameOptions.AvailableLetters, dto.GameOptions.Rounds, dto.GameOptions.NumberOfPlayers, dto.GameOptions.Time));

            await _games.AddAsync(game);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            await Clients.Caller.SendAsync("game_created", new
            {
                ok = true,
                is_admin = true,
                game
            });
        }

        [HubMethodName("game_join")]
        public async Task Join(JoinToGameRoomDto dto)
        {
            User user = await _users.GetByIdAsync(dto.UserId);

            Game game = await _games.GetByIdAsync(dto.GameId);
            var player = game.AddPlayer(user);

            ConnectionBinding.BindConnectionId(Context.ConnectionId, user.Id, game.Id);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            GameState gameState = game.GetCurrentState();

            switch (gameState)
            {
                case GameState.Waiting:

                    await Clients.Caller.SendAsync("im_joined_game", new
                    {
                        ok = true,
                        game = _mapper.Map<Game, GameDto>(game),
                        player = _mapper.Map<Player, PlayerDto>(game.GetPlayer(user.Id))
                    });

                    await Clients.GroupExcept(game.Id.ToString(), Context.ConnectionId).SendAsync("player_joined_game", new
                    {
                        ok = true,
                        player = _mapper.Map<Player, PlayerDto>(game.GetPlayer(user.Id))
                    });

                    break;

                case GameState.InProgress:

                    if (player.IsInRound)
                    {
                        await Clients.Caller.SendAsync("im_reconected_game", new
                        {
                            ok = true,
                            game = _mapper.Map<Game, GameDto>(game),
                            player = _mapper.Map<Player, PlayerDto>(game.GetPlayer(user.Id)),
                            sortedLetter = game.GetCurrentRoundSortedLetter()
                        });
                    }

                    break;

                case GameState.ThemesValidations:

                    if (player.IsInRound)
                    {
                        var playerValidations = await _validationStorage.GetValidationsAsync(game.Id, game.CurrentRound.Number);

                        if (!playerValidations.Any())
                        {
                            var answers = await _answersStorage.GetPlayersAnswersAsync(game.Id, game.CurrentRound.Number);
                            var defaultValidationsForPlayer = answers.BuildValidationsForPlayer(user.Id);

                            await Clients.Caller.SendAsync("im_reconected_game", new
                            {
                                ok = true,
                                game = _mapper.Map<Game, GameDto>(game),
                                validations = defaultValidationsForPlayer
                            });
                        }
                    }

                    break;

                case GameState.Finished:

                    Guid[] gameWinners = await GetWinners(game);

                    await Clients.Caller.SendAsync("im_reconected_game", new
                    {
                        game = _mapper.Map<Game, GameDto>(game),
                        winners = gameWinners
                    });

                    break;
            }
        }

        private async Task<Guid[]> GetWinners(Game game)
        {
            var playersPontuations = await _pontuationStorage.GetPontuationsAsync(game.Id);
            var gameWinners = playersPontuations.GetWinners();
            return gameWinners;
        }

        [HubMethodName("game_start_round")]
        public async Task StartNextRound(StartGameDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameRoomId);
            game.StartNextRound();

            IClientProxy connectionGroup = Clients.Group(game.Id.ToString());
            await connectionGroup.SendAsync("game_round_started", new
            {
                ok = true,
                id = game.Id,
                sortedLetter = game.GetCurrentRoundSortedLetter()
            });

            int limitTime = game.Options.Time;
            _gameTimer.StartRoundTimer(game.Id, limitTime, async (gameId, currentTime) =>
            {
                await _hubContext.Group(gameId).SendAsync("game_answers_time_elapsed", currentTime);
            },
            async (gameId) =>
            {
                StopRound(game);
                await _hubContext.Group(gameId).SendAsync("game_stop", new
                {
                    ok = true,
                    reason = "time_over"
                });
            });
        }

        [HubMethodName("game_stop")]
        public async Task CallStop(CallStopDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameId);

            var playerCalledStop = game.Players.FirstOrDefault(p => p.Id == dto.UserId);

            await Clients.Group(game.Id).SendAsync("game_stop", new
            {
                ok = true,
                reason = "player_call_stop",
                userName = playerCalledStop.User.UserName
            });

            StopRound(game);
        }

        [HubMethodName("send_answers")]
        public async Task SendAnswers(RoundAnswers answers)
        {
            await _answersStorage.AddAsync(answers);

            await Clients.Caller.SendAsync("im_send_answers", new
            {
                ok = true
            });
        }

        [HubMethodName("send_validations")]
        public async Task SendThemeAnswersValidation(RoundValidations validations)
        {
            await _validationStorage.AddAsync(validations);

            await Clients.Caller.SendAsync("im_send_validations", new
            {
                ok = true
            });

            // if (game.AllPlayersSendValidationsOfTheme(dto.Validation.Theme))
            // {
            //     _timerService.StopValidationTimer(game.Id);
            //     await GoToNextStep(game);
            // }
        }

        [HubMethodName("player_change_status")]
        public async Task ChangePlayerStatus(ChangePlayerStatusDto dto)
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

        private void StopRound(Game game)
        {
            _gameTimer.StopRoundTimer(game.Id);
            _gameTimer.StartSendAnswersTime(game.Id, (gameId, elapsedTime) => { }, async (id) =>
            {
                await OnSendAnswersTimeOver(game);
            });
        }

        private async Task OnSendAnswersTimeOver(Game game)
        {
            await _hubContext.Group(game.Id).SendAsync("send_answers_time_over");
            game.BeginCurrentRoundThemesValidations();
        }

        private async Task CalculateRoundPontuation(Game game)
        {
            await _roundScorer.ProcessRoundPontuationAsync(game.Id, game.CurrentRound.Number);
        }

        private async Task FinishRound(Game game)
        {
            var scoreBoard = await _pontuationStorage.GetPontuationsAsync(game.Id, game.CurrentRound.Number);

            game.FinishRound();
            await _hubContext.Group(game.Id).SendAsync("game_round_finished", new
            {
                ok = true,
                scoreBoard
            });

        }

        private async Task FinishGame(Game game)
        {
            var winners = GetWinners(game);

            game.Finish();
            await _hubContext.Group(game.Id).SendAsync("game_end", new
            {
                ok = true,
                winners
            });
        }
    }
}