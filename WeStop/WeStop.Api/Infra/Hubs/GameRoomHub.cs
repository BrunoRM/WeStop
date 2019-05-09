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
            Pontuations = new List<PlayerPontuation>();
            IsReady = false;
            IsAdmin = false;
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsReady { get; set; }
        public ICollection<PlayerPontuation> Pontuations { get; set; }
    }

    class RoundPontuation
    {
        public Round Round { get; set; }
        public int Points { get; set; }
    }

    class PlayerPontuation
    {
        public PlayerPontuation()
        {
            RoundsPontuations = new List<RoundPontuation>();
        }

        public ICollection<RoundPontuation> RoundsPontuations { get; set; }
        public int TotalPontuation => RoundsPontuations.Sum(x => x.Points);
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
        // private readonly WeStopDbContext _db;
        // private readonly IMapper _mapper;
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

        public GameRoomHub(WeStopDbContext db, IMapper mapper)
        {
            // _db = db;
            // _mapper = mapper;
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

            if (game is null)
                return;

            if (game.Players.Any(x => x.Id == user.Id))
            {
                var gamePlayer = game.Players.FirstOrDefault(x => x.Id == user.Id);

                if (gamePlayer.IsAdmin)
                {
                    await Clients.Caller.SendAsync("joinedToGame", new
                    {
                        ok = true,
                        is_admin = gamePlayer.IsAdmin,
                        game
                    });
                }
                else
                    await Clients.Caller.SendAsync("error");
            }
            else
            {
                var playerJoined = new Player
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    IsAdmin = false
                };

                game.Players.Add(playerJoined);

                await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

                await Clients.Caller.SendAsync("joinedToGame", new
                {
                    ok = true,
                    is_admin = false,
                    game
                });

                await Clients.GroupExcept(game.Id.ToString(), Context.ConnectionId).SendAsync("playerJoinedToGame", new
                {
                    ok = true,
                    player = playerJoined
                });
            }
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

            await Clients.Group(game.Id.ToString()).SendAsync("gameStarted", new { ok = true, gameRoomConfig = new { game.Id, themes = game.Options.Themes, currentRound = game.Rounds.Last() } });
        }

        [HubMethodName("player.ready")]
        public async Task PlayerReady(Guid gameId, string userName)
        {
            var player = GetPlayerInGame(gameId, userName);


        }

        [HubMethodName("player.notReady")]
        public async Task PlayerNotReady(Guid gameId, string userName)
        {

        }

        private Player GetPlayerInGame(Guid gameId, string userName)
        {
            var game = _games[gameId];

            if (game is null)
                return null;

            return game.Players.FirstOrDefault(x => x.UserName == userName);
        }
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
