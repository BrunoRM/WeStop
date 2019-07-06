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
    public class GameRoomHub : Hub
    {
        private readonly IUserStorage _users;
        private readonly IGameStorage _games;
        private readonly Timers _timers;
        private static IDictionary<string, (Guid gameId, Guid playerId)> _connectionsInfo = new Dictionary<string, (Guid gameId, Guid playerId)>();

        public GameRoomHub(IUserStorage userStorage, IGameStorage gameStorage, Timers timers)
        {
            _users = userStorage;
            _games = gameStorage;
            _timers = timers;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            /// Buscar o jogo em que o jogador está, e mudar o status dele para offline
            var game = await _games.GetByIdAsync(_connectionsInfo[Context.ConnectionId].gameId);
            game.GetPlayer(_connectionsInfo[Context.ConnectionId].playerId).SetOffline();

            _connectionsInfo.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("games.create")]
        public async Task CreateGame(CreateGameDto dto)
        {
            var user = await _users.GetByIdAsync(dto.UserId);

            Game game = new Game(dto.Name, string.Empty, new GameOptions(dto.GameOptions.Themes, dto.GameOptions.AvailableLetters, dto.GameOptions.Rounds, dto.GameOptions.NumberOfPlayers, dto.GameOptions.RoundTime));
            game.AddPlayer(new Player(user, true));

            await _games.CreateAsync(game);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            await Clients.Caller.SendAsync("game.created", new
            {
                ok = true,
                is_admin = true,
                game
            });
        }

        [HubMethodName("game.join")]
        public async Task Join(JoinToGameRoomDto dto)
        {
            var user = await _users.GetByIdAsync(dto.UserId);

            var game = await _games.GetByIdAsync(dto.GameId);

            var player = game.GetPlayer(user.Id);

            if (player is null)
            {
                player = new Player(user, false);
                game.AddPlayer(player);
            }
            else
                player.SetOnline();

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            _connectionsInfo.Add(Context.ConnectionId, (gameId: game.Id, playerId: player.Id));

            await Clients.Caller.SendAsync("game.player.joined", new
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

            await Clients.GroupExcept(game.Id.ToString(), Context.ConnectionId).SendAsync("game.players.joined", new
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

        [HubMethodName("game.startRound")]
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
            await connectionGroup.SendAsync("game.roundStarted", new
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
                await hub.GetGameGroup(gameId).SendAsync("roundTimeElapsed", currentTime);
            },
            async (gameId, hub) =>
            {
                await hub.GetGameGroup(gameId).SendAsync("players.stopCalled", new
                {
                    ok = true,
                    reason = "TIME_OVER"
                });

                Stop(game);
            });
        }

        [HubMethodName("players.stop")]
        public async Task CallStop(CallStopDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameId);

            var playerCalledStop = game.Players.FirstOrDefault(x => x.User.Id == dto.UserId);

            var connectionGroup = Clients.Group(dto.GameId.ToString());

            await connectionGroup.SendAsync("players.stopCalled", new
            {
                ok = true,
                reason = "player_call_stop",
                userName = playerCalledStop.User.UserName
            });

            Stop(game);
        }

        [HubMethodName("player.sendAnswers")]
        public async Task SendAnswers(SendAnswersDto dto)
        {
            Game game = await _games.GetByIdAsync(dto.GameId);
            Player player = game.GetPlayer(dto.UserId);
            game.AddPlayerAnswers(player.Id, dto.Answers);

            await Clients.Group(game.Id.ToString()).SendAsync("answers_received");
        }

        [HubMethodName("player.sendAnswersValidations")]
        public async Task SendThemeAnswersValidation(SendThemeAnswersValidationDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameId);

            var player = game.Players.FirstOrDefault(x => x.User.Id == dto.UserId);

            game.AddPlayerAnswersValidations(player.Id, new ThemeValidation(dto.Validation.Theme, dto.Validation.AnswersValidations));

            await Clients.Caller.SendAsync("player.themeValidationsReceived", new
            {
                ok = true,
                dto.Validation.Theme
            });
        }

        [HubMethodName("player.changeStatus")]
        public async Task ChangePlayerStatus(ChangePlayerStatusDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameId);

            var player = game.Players.FirstOrDefault(x => x.User.Id == dto.UserId);
            player.ChangeStatus(dto.IsReady);

            await Clients.GroupExcept(dto.GameId.ToString(), Context.ConnectionId).SendAsync("player.statusChanged", new
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
                await hubContext.Clients.Group(id.ToString()).SendAsync("answers_time_over");
                foreach (var connectionInfo in _connectionsInfo)
                {
                    var validations = game.BuildValidationForPlayer(connectionInfo.Value.playerId);
                    await hubContext.Clients.Client(connectionInfo.Key).SendAsync("all_answers_received", validations);
                }

                _timers.StartValidationTimer(game.Id, async (gameId, hub, elapsedTime) =>
                {
                    await hub.GetGameGroup(gameId).SendAsync("validation_time_elapsed", elapsedTime);
                }, async (gameId, hub) =>
                {
                    await hub.GetGameGroup(gameId).SendAsync("validation_time_over");

                    string[] themes = game.GetThemes();
                    foreach (var theme in themes)
                    {
                        game.GeneratePontuationForTheme(theme);
                    }

                    if (game.IsFinalRound())
                    {
                        await hub.GetGameGroup(gameId).SendAsync("game.end", new
                        {
                            ok = true,
                            winners = game.GetWinners(),
                            scoreboard = game.GetScoreboard()
                        });
                    }
                    else
                    {
                        await hub.GetGameGroup(gameId).SendAsync("game.roundFinished", new
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