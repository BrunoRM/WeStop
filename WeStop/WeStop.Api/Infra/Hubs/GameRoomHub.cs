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
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public GameOptions Options { get; set; }
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
        public Guid Id { get; set; }
        public bool IsAdmin { get; set; }
        public ICollection<PlayerPontuation> Pontuation { get; set; }
    }

    class PlayerPontuation
    {
        public int Round { get; set; }
        public int Points { get; set; }
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

        public async Task CreateGame(CreateGameDto dto)
        {
            var gameInfo = new Game
            {
                Id = new Guid(),
                Name = dto.Name,
                Options = new GameOptions
                {
                    Themes = dto.GameOptions.Themes,
                    AvailableLetters = dto.GameOptions.AvailableLetters,
                    NumberOfPlayers = dto.GameOptions.NumberOfPlayers,
                    Rounds = dto.GameOptions.Rounds
                }
            };

            _games.Add(gameInfo.Id, gameInfo);
        }

        //public async Task CreateGame(CreateGameDto data)
        //{
        //    var gameRoomPlayer = await _db.GameRoomPlayers
        //        .Include(x => x.Player)
        //        .Include(x => x.GameRoom)
        //        .FirstOrDefaultAsync(x => x.GameRoomId == data.GameRoomId);

        //    if (gameRoomPlayer is null)
        //        await Clients.Caller.SendAsync("error", new { ok = false, error = "" });

        //    if (gameRoomPlayer.IsAdmin)

        //}

        [HubMethodName("join")]
        public async Task Join(JoinToGameRoomDto data)
        {
            var player = await _db.Players.FirstOrDefaultAsync(x => x.Id == data.PlayerId);

            var gameRoomToJoin = await _db.GameRooms
                .Include(x => x.Players)
                .FirstOrDefaultAsync(x => x.Id == data.GameRoomId);

            if (gameRoomToJoin is null)
                await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.GameRoomNotFound });

            var gameRoomPlayerIsAlready = (await _db.GameRoomPlayers
                .Include(x => x.GameRoom)
                .FirstOrDefaultAsync(x => x.PlayerId == player.Id)).GameRoom;

            if (gameRoomPlayerIsAlready != null && gameRoomPlayerIsAlready.Id != gameRoomToJoin.Id)
                await Clients.Caller.SendAsync("error", new { ok = false, alread_in_gameRoom = true, gameRoom = _mapper.Map<GameRoom, GameRoomDto>(gameRoomPlayerIsAlready) });

            //if (!string.IsNullOrEmpty(gameRoomToJoin.Password))
            //    await ValidatePassword(data.Password, player, gameRoomToJoin);

            await ConnectToGameroom(player, gameRoomToJoin);
        }
        
        public class StartGameDto
        {
            public Guid PlayerId { get; set; }
            public Guid GameRoomId { get; set; }
        }

        [HubMethodName("startGame")]
        public async Task StartGame(StartGameDto dto)
        {
            var gameRoom = await _db.GameRooms
                .Include(x => x.Players)
                .FirstOrDefaultAsync(x => x.Id == dto.GameRoomId);

            if (gameRoom is null)
                await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.GameRoomNotFound });

            if (!gameRoom.Players.FirstOrDefault(x => x.PlayerId == dto.PlayerId).IsAdmin)
                await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.NotAdmin });

            if (gameRoom.Players.Count() < 2)
                await Clients.Caller.SendAsync("error", new { ok = false, error = "insuficient_players" });

            var availableLetters = gameRoom.AvailableLetters.Split(",", StringSplitOptions.RemoveEmptyEntries);

            string sortedLetter = availableLetters[new Random().Next(0, availableLetters.Length - 1)];
            var themes = gameRoom.Themes.Split(",", StringSplitOptions.RemoveEmptyEntries);

            await Clients.Group(gameRoom.Id.ToString()).SendAsync("gameStarted", new { ok = true, gameRoomConfig = new { gameRoom.Id, sortedLetter, themes } });
        }

        private async Task ValidatePassword(string password, Domain.Player player, GameRoom gameRoom)
        {
            if (string.IsNullOrEmpty(password))
                await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.InvalidPasswordRequired });

            var hashGenerator = new MD5HashGenerator();
            string passwordHash = hashGenerator.GetMD5Hash(password);

            if (hashGenerator.VerifyMd5Hash(passwordHash, gameRoom.Password))
                await ConnectToGameroom(player, gameRoom);
        }

        //public class UserProvider : IUserIdProvider
        //{
        //    public string GetUserId(HubConnectionContext connection)
        //    {
        //        connection.
        //        throw new NotImplementedException();
        //    }
        //}

        private async Task ConnectToGameroom(Domain.Player player, GameRoom gameRoom)
        {
            var playerIsAdmin = gameRoom.Players.FirstOrDefault(x => x.PlayerId == player.Id).IsAdmin;

            await Groups.AddToGroupAsync(Context.ConnectionId, gameRoom.Id.ToString());

            await Clients.Caller.SendAsync("connected", new { ok = true, is_admin = playerIsAdmin, gameRoom = _mapper.Map<GameRoom, GameRoomDto>(gameRoom) });
        }
    }

    public class ConnectToGameRoom
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
    }

    public class JoinToGameRoomDto
    {
        public Guid PlayerId { get; set; }
        public Guid GameRoomId { get; set; }
    }
}
