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
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
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
        private readonly WeStopDbContext _db;
        private readonly IMapper _mapper;
        private static IDictionary<Guid, Game> _games = new Dictionary<Guid, Game>();
        private static IDictionary<Guid, Player> _userTracking = new Dictionary<Guid, Player>();

        public GameRoomHub(WeStopDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
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
            var player = await _db.Players.FirstOrDefaultAsync(x => x.UserName == dto.UserName);

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
                Players = { new Player { Id = player.Id, UserName = player.UserName, IsAdmin = true } }
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
            var player = await _db.Players.FirstOrDefaultAsync(x => x.UserName == data.UserName);

            var game = _games[data.GameRoomId];

            if (game is null)
                return;

            if (game.Players.Any(x => x.Id == player.Id))
            {
                var gamePlayer = game.Players.FirstOrDefault(x => x.Id == player.Id);

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
                game.Players.Add(new Player
                {
                    Id = player.Id,
                    UserName = player.UserName,
                    IsAdmin = false
                });

                await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

                await Clients.Caller.SendAsync("joinedToGame", new
                {
                    ok = true,
                    is_admin = false,
                    game
                });
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("sendMessageToGroup")]
        public async Task SendMessageToGroup(Guid gameId)
        {
            await Clients.Group(gameId.ToString()).SendAsync("groupMessage", "Oláaaaaa");
        }
        
        public class StartGameDto
        {
            public string UserName { get; set; }
            public Guid GameRoomId { get; set; }
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

            await Clients.Group(game.Id.ToString()).SendAsync("gameStarted", new { ok = true, gameRoomConfig = new { game.Id, themes = game.Options.Themes, round = game.Rounds.Last() } });
        }

        //private async Task ValidatePassword(string password, Domain.Player player, GameRoom gameRoom)
        //{
        //    if (string.IsNullOrEmpty(password))
        //        await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.InvalidPasswordRequired });

        //    var hashGenerator = new MD5HashGenerator();
        //    string passwordHash = hashGenerator.GetMD5Hash(password);

        //    if (hashGenerator.VerifyMd5Hash(passwordHash, gameRoom.Password))
        //        await ConnectToGameroom(player, gameRoom);
        //}

        //public class UserProvider : IUserIdProvider
        //{
        //    public string GetUserId(HubConnectionContext connection)
        //    {
        //        connection.
        //        throw new NotImplementedException();
        //    }
        //}
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
