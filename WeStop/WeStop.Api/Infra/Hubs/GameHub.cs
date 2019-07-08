using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Classes;
using WeStop.Api.Dtos;
using WeStop.Api.Extensions;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Hubs
{
    public class GameHub : Hub
    {
        private readonly IUserStorage _users;
        private readonly IGameStorage _games;
        private readonly IPlayerConnectionStorage _playerConnectionStorage;
        private readonly Timers _timers;

        public GameHub(IUserStorage userStorage, IGameStorage gameStorage, IPlayerConnectionStorage playerConnectionStorage, Timers timers)
        {
            _users = userStorage;
            _games = gameStorage;
            _playerConnectionStorage = playerConnectionStorage;
            _timers = timers;
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

            Game game = new Game(dto.Name, string.Empty, new GameOptions(dto.GameOptions.Themes, dto.GameOptions.AvailableLetters, dto.GameOptions.Rounds, dto.GameOptions.NumberOfPlayers, dto.GameOptions.RoundTime));
            game.AddPlayer(new Player(user, true));

            await _games.CreateAsync(game);

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

            Player player = game.GetPlayer(user.Id);

            if (player is null)
            {
                player = new Player(user, false);
                game.AddPlayer(player);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            PlayerConnection playerConnection = new PlayerConnection(Context.ConnectionId, player.Id, game.Id);
            await _playerConnectionStorage.AddAsync(playerConnection);

            await Clients.Caller.SendAsync("game_joined", new
            {
                ok = true,
                game = new
                {
                    game.Id,
                    game.Name,
                    game.Options.NumberOfPlayers,
                    game.Options.Rounds,
                    game.Options.RoundTime,
                    game.Options.Themes,
                    currentRound = game.GetNextRoundNumber(),
                    players = game.Players.Select(p => new
                    {
                        p.User.Id,
                        p.User.UserName,
                        p.IsReady
                    }),
                    scoreboard = game.GetScoreboard()
                },
                player = new
                {
                    player.User.Id,
                    player.User.UserName,
                    player.IsAdmin,
                    player.IsReady,
                    player.EarnedPoints
                }
            });

            await Clients.GroupExcept(game.Id.ToString(), Context.ConnectionId).SendAsync("game_player_joined", new
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

        [HubMethodName("game_start_round")]
        public async Task StartNextRound(StartGameDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameRoomId);

            if (game is null)
                await Clients.Caller.SendAsync("error", new { ok = false, error = "GAME_NOT_FOUND" });

            if (!game.IsPlayerAdmin(dto.UserId))
                await Clients.Caller.SendAsync("error", new { ok = false, error = "NOT_ADMIN" });

            if (!game.HasSuficientPlayersToStartNewRound())
                await Clients.Caller.SendAsync("error", new { ok = false, error = "insuficient_players" });

            game.StartNextRound();

            IClientProxy connectionGroup = Clients.Group(game.Id.ToString());
            await connectionGroup.SendAsync("game_round_started", new
            {
                ok = true,
                gameRoomConfig = new
                {
                    game.Id,
                    themes = game.Options.Themes,
                    currentRound = game.Rounds.Last()
                }
            });

            int limitTime = game.Options.RoundTime;
            _timers.StartRoundTimer(game.Id, limitTime, async (gameId, hub, currentTime) =>
            {
                await hub.Group(gameId).SendAsync("game_answers_time_elapsed", currentTime);
            },
            async (gameId, hub) =>
            {
                await hub.Group(gameId).SendAsync("game_stop", new
                {
                    ok = true,
                    reason = "time_over"
                });

                Stop(game);
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

            Stop(game);
        }

        [HubMethodName("player_send_answers")]
        public async Task SendAnswers(SendAnswersDto dto)
        {
            Game game = await _games.GetByIdAsync(dto.GameId);
            Player player = game.GetPlayer(dto.UserId);

            foreach (var themeAnswer in dto.Answers)
            {
                string theme = themeAnswer.Theme;
                string answer = themeAnswer.Answer;
                game.AddPlayerAnswerForTheme(player.Id, theme, answer);
            }

            await Clients.Group(game.Id.ToString()).SendAsync("answers_received");
        }

        [HubMethodName("player_send_validations")]
        public async Task SendThemeAnswersValidation(SendThemeAnswersValidationDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameId);

            var player = game.Players.FirstOrDefault(x => x.User.Id == dto.UserId);

            game.AddPlayerAnswersValidations(player.Id, new ThemeValidation(dto.Validation.Theme, dto.Validation.AnswersValidations));

            await Clients.Caller.SendAsync("player_validations_sended", new
            {
                ok = true,
                dto.Validation.Theme
            });
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
        }

        private void Stop(Game game)
        {
            _timers.StopRoundTimer(game.Id);
            _timers.StartSendAnswersTime(game.Id, (gameId, hub, elapsedTime) => { }, async (id, hubContext) =>
            {
                await hubContext.Group(id).SendAsync("send_answers_time_over");

                var connectionsOfGame = await _playerConnectionStorage.GetConnectionsForGameAsync(game.Id);
                foreach (var playerConnection in connectionsOfGame)
                {
                    var validations = game.BuildValidationForPlayer(playerConnection.PlayerId);
                    await hubContext.Clients.Client(playerConnection.ConnectionId).SendAsync("players_answers_received", validations);
                }

                _timers.StartValidationTimer(game.Id, async (gameId, hub, elapsedTime) =>
                {
                    await hub.Group(gameId).SendAsync("validation_time_elapsed", elapsedTime);
                }, async (gameId, hub) =>
                {
                    await hub.Group(gameId).SendAsync("validation_time_over");

                    string[] themes = game.GetThemes();
                    foreach (var theme in themes)
                    {
                        game.GeneratePontuationForTheme(theme);
                    }

                    if (game.IsFinalRound())
                    {
                        await hub.Group(gameId).SendAsync("game_end", new
                        {
                            ok = true,
                            winners = game.GetWinners(),
                            scoreboard = game.GetScoreboard()
                        });
                    }
                    else
                    {
                        await hub.Group(gameId).SendAsync("game_round_finished", new
                        {
                            ok = true,
                            scoreboard = game.GetScoreboard()
                        });
                    }
                });
            });
        }
    }
}