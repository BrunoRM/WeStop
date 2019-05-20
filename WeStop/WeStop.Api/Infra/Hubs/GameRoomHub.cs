using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Classes;
using WeStop.Api.Dtos;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Hubs
{
    public class GameRoomHub : Hub
    {
        private readonly IUserStorage _users;
        private readonly IGameStorage _games;
        
        public GameRoomHub(IUserStorage userStorage, IGameStorage gameStorage)
        {
            _users = userStorage;
            _games = gameStorage;
        }

        [HubMethodName("games.create")]
        public async Task CreateGame(CreateGameDto dto)
        {
            var user = await _users.GetByIdAsync(dto.UserId);

            var game = new Game(dto.Name, string.Empty, new GameOptions(dto.GameOptions.Themes, dto.GameOptions.AvailableLetters, dto.GameOptions.Rounds, dto.GameOptions.NumberOfPlayers));
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

            var player = game.Players.FirstOrDefault(x => x.User.Id == user.Id);

            if (player is null)
            {
                player = new Player(user, false);
                game.AddPlayer(player);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            await Clients.Caller.SendAsync("game.player.joined", new
            {
                ok = true,
                game = new
                {
                    game.Id,
                    game.Name,
                    game.Options.NumberOfPlayers,
                    game.Options.Rounds,
                    game.Options.Themes,
                    currentRound = game.CurrentRound?.Number + 1 ?? 1,
                    players = game.Players.Select(p => new
                    {
                        p.User.Id,
                        p.User.UserName,
                        p.IsReady
                    })
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
        public async Task StartGame(StartGameDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameRoomId);

            if (game is null)
                await Clients.Caller.SendAsync("error", new { ok = false, error = "GAME_NOT_FOUND" });

            if (!game.Players.FirstOrDefault(x => x.User.Id == dto.UserId).IsAdmin)
                await Clients.Caller.SendAsync("error", new { ok = false, error = "NOT_ADMIN" });

            if (game.Players.Count() < 2)
                await Clients.Caller.SendAsync("error", new { ok = false, error = "insuficient_players" });

            game.StartNextRound();

            await Clients.Group(game.Id.ToString()).SendAsync("game.roundStarted", new
            {
                ok = true,
                gameRoomConfig = new
                {
                    game.Id,
                    themes = game.Options.Themes,
                    currentRound = game.Rounds.Last()
                }
            });
        }

        [HubMethodName("players.stop")]
        public async Task Stop(CallStopDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameId);

            var playerCalledStop = game.Players.FirstOrDefault(x => x.User.Id == dto.UserId);

            await Clients.Group(dto.GameId.ToString()).SendAsync("players.stopCalled", new
            {
                ok = true,
                userName = playerCalledStop.User.UserName
            });
        }

        [HubMethodName("player.sendAnswers")]
        public async Task SendAnswers(SendAnswersDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameId);

            var player = game.Players.FirstOrDefault(x => x.User.Id == dto.UserId);

            game.GetPlayerCurrentRound(player.User.Id).AddAnswers(dto.Answers);

            var playerAnswers = game.GetPlayerCurrentRound(player.User.Id).Answers;

            var answers = new Dictionary<string, string>();

            await Clients.GroupExcept(dto.GameId.ToString(), Context.ConnectionId).SendAsync("player.answersSended", new
            {
                ok = true,
                answers = playerAnswers
            });
        }

        [HubMethodName("player.sendAnswersValidations")]
        public async Task SendThemeAnswersValidation(SendThemeAnswersValidationDto dto)
        {
            var game = await _games.GetByIdAsync(dto.GameId);

            var player = game.Players.FirstOrDefault(x => x.User.Id == dto.UserId);

            game.GetPlayerCurrentRound(player.User.Id).AddThemeAnswersValidations(new ThemeValidation(dto.Validation.Theme, dto.Validation.AnswersValidations));

            await Clients.Caller.SendAsync("player.themeValidationsReceived", new
            {
                ok = true,
                dto.Validation.Theme
            });

            // Se todos os jogadores ja enviaram as validações para esse tema, a pontuação já pode ser processada
            if (game.AllPlayersSendValidationsOfTheme(dto.Validation.Theme))
                game.ProccessPontuationForTheme(dto.Validation.Theme);

            if (game.AllPlayersSendValidationsOfAllThemes())
            {
                if (game.IsFinalRound())
                {
                    await Clients.Group(dto.GameId.ToString()).SendAsync("game.end", new
                    {
                        ok = true,
                        finalScoreboard = game.GetFinalPontuation()
                    });
                }
                else
                {
                    await Clients.Group(dto.GameId.ToString()).SendAsync("game.roundFinished", new
                    {
                        ok = true,
                        scoreboard = game.GetScoreboard()
                    });
                }
            }
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
    }
}
