using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Application.Dtos.GameRoom;
using WeStop.Domain;
using WeStop.Domain.Errors;
using WeStop.Helpers.Criptography;
using WeStop.Infra;

namespace WeStop.Api.Infra.Hubs
{
    class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
    }

    class Game
    {
        public Game()
        {
            Players = new List<Player>();
            Rounds = new List<Round>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public GameOptions Options { get; set; }
        public ICollection<Round> Rounds { get; set; }
        public ICollection<Player> Players { get; set; }
    }

    public class GameOptions
    {
        public string[] Themes { get; set; }
        public string[] AvailableLetters { get; set; }
        public int Rounds { get; set; }
        public int NumberOfPlayers { get; set; }
    }

    class Player
    {
        public Player()
        {
            Rounds = new Dictionary<int, PlayerRound>();
            IsReady = false;
            IsAdmin = false;
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsReady { get; set; }
        public IDictionary<int, PlayerRound> Rounds { get; set; }
    }

    class PlayerRound
    {
        public IDictionary<string, string> Answers { get; set; }
        public int EarnedPoints { get; set; }
    }

    class Round
    {
        public Round()
        {
            Finished = false;
        }

        public int Number { get; set; }
        public string SortedLetter { get; set; }
        public bool Finished { get; set; }
    }

    public class GameRoomHub : Hub
    {
        private static IDictionary<Guid, Game> _games = new Dictionary<Guid, Game>();
        private static ICollection<User> _users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "Bruno"
            },
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "Gustavo"
            },
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "Giovani"
            },
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "Lucas"
            },
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "Davi"
            }
        };

        public GameRoomHub()
        {
        }

        public class CreateGameDto
        {
            public string UserName { get; set; }
            public string Name { get; set; }
            public GameOptions GameOptions { get; set; }
        }

        [HubMethodName("createGame")]
        public async Task CreateGame(CreateGameDto dto)
        {
            var user = _users.FirstOrDefault(x => x.UserName == dto.UserName);

            var game = new Game
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Options = new GameOptions
                {
                    Themes = dto.GameOptions.Themes,
                    AvailableLetters = dto.GameOptions.AvailableLetters,
                    NumberOfPlayers = dto.GameOptions.NumberOfPlayers,
                    Rounds = dto.GameOptions.Rounds
                },
                Players = { new Player { Id = user.Id, UserName = user.UserName, IsAdmin = true } }
            };

            _games.Add(game.Id, game);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            await Clients.Caller.SendAsync("gameCreated", new
            {
                ok = true,
                is_admin = true,
                game
            });
        }

        [HubMethodName("getGames")]
        public async Task GetGames()
        {
            await Clients.Caller.SendAsync("getGamesResponse", new
            {
                ok = true,
                games = _games.Values.Where(x => x.Players.Count < x.Options.NumberOfPlayers).Select(x => new
                {
                    x.Id,
                    x.Name,
                    numberOfPlayersAccepted = x.Options.NumberOfPlayers,
                    numberOfPlayers = x.Players.Count
                })
            });
        }

        [HubMethodName("join")]
        public async Task Join(JoinToGameRoomDto data)
        {
            var user = _users.FirstOrDefault(x => x.UserName == data.UserName);

            var game = _games[data.GameRoomId];

            var player = game.Players.FirstOrDefault(x => x.Id == user.Id);

            if (player is null)
            {
                player = new Player
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    IsAdmin = false
                };

                game.Players.Add(player);

            }

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            await Clients.Caller.SendAsync("joinedToGame", new
            {
                ok = true,
                player,
                game
            });

            await Clients.GroupExcept(game.Id.ToString(), Context.ConnectionId).SendAsync("playerJoinedToGame", new
            {
                ok = true,
                player
            });
        }        

        [HubMethodName("startGame")]
        public async Task StartGame(StartGameDto dto)
        {
            var game = _games[dto.GameRoomId];

            if (game is null)
                await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.GameRoomNotFound });

            if (!game.Players.FirstOrDefault(x => x.UserName == dto.UserName).IsAdmin)
                await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.NotAdmin });

            if (game.Players.Count() < 2)
                await Clients.Caller.SendAsync("error", new { ok = false, error = "insuficient_players" });

            string sortedLetter = game.Options.AvailableLetters[new Random().Next(0, game.Options.AvailableLetters.Length - 1)];

            game.Rounds.Add(new Round
            {
                Number = 1,
                SortedLetter = sortedLetter
            });

            await Clients.Group(game.Id.ToString()).SendAsync("roundStarted", new
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

        [HubMethodName("stop")]
        public async Task Stop(StopDto dto)
        {
            var game = _games[dto.GameId];

            var playerCalledStop = game.Players.FirstOrDefault(x => x.UserName == dto.UserName);

            await Clients.Group(dto.GameId.ToString()).SendAsync("stopCalled", new
            {
                ok = true,
                userName = playerCalledStop.UserName
            });
        }

        [HubMethodName("player.sendAnswers")]
        public async Task SendAnswers(SendAnswersDto dto)
        {
            var game = _games[dto.GameId];

            var player = game.Players.FirstOrDefault(x => x.UserName == dto.UserName);

            player.Rounds.Add(game.Rounds.Last().Number, new PlayerRound
            {
                Answers = dto.Answers
            });

            var playerAnswers = player.Rounds.FirstOrDefault(x => x.Key == game.Rounds.Last().Number).Value.Answers;

            var answers = new Dictionary<string, string>();

            // Remove os espaços em branco das respostas
            foreach (var key in playerAnswers.Keys)
            {
                if (!string.IsNullOrEmpty(playerAnswers[key]))
                    answers.Add(key, playerAnswers[key].Trim());
            }

            await Clients.GroupExcept(dto.GameId.ToString(), Context.ConnectionId).SendAsync("player.answersReceived", new
            {
                ok = true,
                answers
            });
        }

        [HubMethodName("player.sendThemeAnswersValidation")]
        public async Task SendThemeAnswersValidation(SendThemeAnswersValidationDto dto)
        {

        }

        [HubMethodName("player.changeStatus")]
        public async Task ChangePlayerStatus(ChangePlayerStatusDto dto)
        {
            var player = GetPlayerInGame(dto.GameId, dto.UserName);

            player.IsReady = dto.IsReady;

            await Clients.GroupExcept(dto.GameId.ToString(), Context.ConnectionId).SendAsync("player.statusChanged", new
            {
                ok = true,
                player
            });
        }

        private Player GetPlayerInGame(Guid gameId, string userName) =>
            _games[gameId].Players.FirstOrDefault(x => x.UserName == userName);
    }

    public class SendThemeAnswersValidationDto
    {
        public Guid GameId { get; set; }
        public string UserName { get; set; }
        public ThemeValidationDto Validation { get; set; }

    }
    public class ThemeValidationDto
    {
        public string Theme { get; set; }
        public IDictionary<string, bool> Validations { get; set; }
    }

    public class SendAnswersDto
    {
        public Guid GameId { get; set; }
        public string UserName { get; set; }
        public IDictionary<string, string> Answers { get; set; }
    }

    public class StopDto
    {
        public Guid GameId { get; set; }
        public string UserName { get; set; }
    }

    public class ChangePlayerStatusDto
    {
        public Guid GameId { get; set; }
        public string UserName { get; set; }
        public bool IsReady { get; set; }
    }

    public class StartGameDto
        {
            public string UserName { get; set; }
            public Guid GameRoomId { get; set; }
        }

    public class ConnectToGameRoom
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
    }

    public class JoinToGameRoomDto
    {
        public string UserName { get; set; }
        public Guid GameRoomId { get; set; }
    }
}
