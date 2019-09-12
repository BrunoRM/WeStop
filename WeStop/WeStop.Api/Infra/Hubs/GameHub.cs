using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Classes;
using WeStop.Api.Dtos;
using WeStop.Api.Extensions;
using WeStop.Api.Infra.Services;
using WeStop.Api.Infra.Storages.Interfaces;

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
        private readonly IPlayerConnectionStorage _playerConnectionStorage;
        private readonly ITimerService _timerService;
        private readonly IMapper _mapper;
        private static IDictionary<Guid, string> _themeValidationContext = new Dictionary<Guid, string>();

        public GameHub(IUserStorage userStorage, IGameStorage gameStorage, IAnswerStorage answersStorage, 
            IValidationStorage validationStorage, IPontuationStorage pontuationStorage, 
            IPlayerConnectionStorage playerConnectionStorage, IHubContext<GameHub> hubContext, 
            ITimerService timers, IMapper mapper)
        {
            _users = userStorage;
            _games = gameStorage;
            _answersStorage = answersStorage;
            _validationStorage = validationStorage;
            _pontuationStorage = pontuationStorage;
            _hubContext = hubContext;
            _playerConnectionStorage = playerConnectionStorage;
            _timerService = timers;
            _mapper = mapper;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _playerConnectionStorage.DeleteAsync(Context.ConnectionId);
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
            game.AddPlayer(user, out string operationMessage);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            PlayerConnection playerConnection = new PlayerConnection(Context.ConnectionId, user.Id, game.Id);
            await _playerConnectionStorage.AddAsync(playerConnection);
                       
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

                    await Clients.Caller.SendAsync("im_reconected_game", new
                    {
                        ok = true,
                        game = _mapper.Map<Game, GameDto>(game),
                        player = _mapper.Map<Player, PlayerDto>(game.GetPlayer(user.Id)),
                        sortedLetter = game.GetCurrentRoundSortedLetter()
                    });

                    break;

                case GameState.ThemesValidations:

                    string themeBeingValidated = _themeValidationContext[game.Id];
                    bool hasPlayerValidatedTheme = game.HasPlayerValidatedTheme(player.Id, themeBeingValidated);

                    ThemeValidation themeValidation = null;
                    if (!hasPlayerValidatedTheme)
                    {
                        themeValidation = game.GetDefaultValidationsOfThemeForPlayer(themeBeingValidated, player.Id);
                    }

                    await Clients.Caller.SendAsync("im_reconected_game", new
                    {
                        ok = true,
                        game = _mapper.Map<Game, GameDto>(game),
                        player = _mapper.Map<Player, PlayerDto>(player),
                        themeBeingValidated,
                        validated = hasPlayerValidatedTheme,
                        themeValidations = themeValidation
                    });

                    break;

                case GameState.Finished:

                    var gameScoreboard = await _pontuationStorage.GetGameScoreboard(game.Id);
                    var gameWinners = gameScoreboard.GetWinners();

                    await Clients.Caller.SendAsync("im_reconected_game", new
                    {
                        game = _mapper.Map<Game, GameDto>(game),
                        winners = gameWinners
                    });

                    break;
            }
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
            _timerService.StartRoundTimer(game.Id, limitTime, async (gameId, currentTime) =>
            {
                await _hubContext.Group(gameId).SendAsync("game_answers_time_elapsed", currentTime);
            },
            async (gameId) =>
            {
                Stop(game);
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
        public async Task SendAnswers(SendAnswersDto dto)
        {
            Game game = await _games.GetByIdAsync(dto.GameId);
            Player player = game.GetPlayer(dto.UserId);

            var roundAnswers = new RoundAnswers(game.Id, dto.RoundNumber, dto.Answers);
            await _answersStorage.AddAsync(roundAnswers);

            await Clients.Caller.SendAsync("im_send_answers", new
            {
                ok = true,
                gameId = game.Id
            });
        }

        [HubMethodName("send_validations")]
        public async Task SendThemeAnswersValidation(SendThemeAnswersValidationDto dto)
        {
            var roundValidations = new RoundValidations(dto.GameId, dto.RoundNumber, dto.Validations);
            await _validationStorage.AddAsync(roundValidations);

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

        // TODO: gerar um evento de resposta somente para o caller ( im_change_status )
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
        }

        private void Stop(Game game)
        {
            _timerService.StopRoundTimer(game.Id);
            _timerService.StartSendAnswersTime(game.Id, (gameId, elapsedTime) => { }, async (id) =>
            {                
                await OnSendAnswersTimeOver(game);
            });
        }

        private async Task OnSendAnswersTimeOver(Game game)
        {
            await _hubContext.Group(game.Id).SendAsync("send_answers_time_over");
            game.BeginCurrentRoundThemesValidations();
            await StartValidationForNextTheme(game);
        }

        private async Task StartValidationForNextTheme(Game game)
        {
            string nextThemeForValidate = game.GetThemeNotValidatedYet();

            var connectionsOfGame = await _playerConnectionStorage.GetConnectionsForGameAsync(game.Id);
            foreach (var playerConnection in connectionsOfGame)
            {
                ThemeValidation validationsOfThemeForPlayer = game.GetDefaultValidationsOfThemeForPlayer(nextThemeForValidate, playerConnection.PlayerId);
                await _hubContext.Clients.Client(playerConnection.ConnectionId).SendAsync("validation_for_theme_started", validationsOfThemeForPlayer);
            }

            AddOrUpdateThemeBeingValidatedForGame(game.Id, nextThemeForValidate);
            _timerService.StartValidationTimerForTheme(game.Id, nextThemeForValidate, async (gameId, theme, elapsedTime) =>
            {
                await OnValidationTimeForThemeElapsed(gameId, elapsedTime);
            }, async (gameId, theme) =>
            {
                await OnValidationTimeForThemeOver(game, theme);
            });
        }

        private void CalculateRoundPontuation(Game game)
        {
            string[] themes = game.GetThemes();
            foreach (var theme in themes)
            {
                game.GeneratePontuationForTheme(theme);
            }
        }

        private async Task OnValidationTimeForThemeElapsed(Guid gameId, int elapsedTime)
        {
            await _hubContext.Group(gameId).SendAsync("validation_time_elapsed", elapsedTime);
        }

        private async Task OnValidationTimeForThemeOver(Game game, string theme)
        {
            await _hubContext.Group(game.Id).SendAsync("validation_time_for_theme_over", theme);
            game.SetDefaultThemeValidationsForPlayersThatHasNotAnyValidations(theme);
            await GoToNextStep(game);
        }

        private async Task GoToNextStep(Game game)
        {
            if (!game.IsAllThemesValidated())
            {
                await StartValidationForNextTheme(game);
            }
            else
            {
                CalculateRoundPontuation(game);
                if (game.IsFinalRound())
                {
                    await FinishGame(game);
                }
                else
                {
                    await FinishRound(game);
                }
            }
        }

        private async Task FinishRound(Game game)
        {
            var scoreBoard = game.GetScoreboard();

            await _hubContext.Group(game.Id).SendAsync("game_round_finished", new
            {
                ok = true,
                scoreBoard = _mapper.Map<ICollection<PlayerScore>, ICollection<PlayerScoreDto>>(scoreBoard)
            });

            game.FinishRound();
        }

        private async Task FinishGame(Game game)
        {
            var scoreBoard = game.GetScoreboard();

            await _hubContext.Group(game.Id).SendAsync("game_end", new
            {
                ok = true,
                winners = game.GetWinners(),
                scoreBoard = _mapper.Map<ICollection<PlayerScore>, ICollection<PlayerScoreDto>>(scoreBoard)
            });

            game.Finish();
        }

        private void AddOrUpdateThemeBeingValidatedForGame(Guid gameId, string theme)
        {
            if (_themeValidationContext.ContainsKey(gameId))
            {
                _themeValidationContext[gameId] = theme;
            }
            else
            {
                _themeValidationContext.Add(gameId, theme);
            }
        }
    }
}