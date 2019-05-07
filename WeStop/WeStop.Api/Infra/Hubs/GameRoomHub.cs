using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WeStop.Application.Dtos.GameRoom;
using WeStop.Domain;
using WeStop.Domain.Errors;
using WeStop.Helpers.Criptography;
using WeStop.Infra;

namespace WeStop.Api.Infra.Hubs
{
    public class GameRoomHub : Hub
    {
        private readonly WeStopDbContext _db;
        private readonly IMapper _mapper;

        public GameRoomHub(WeStopDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("join")]
        public async Task Join(JoinToGameRoom data)
        {
            var player = await _db.Players.FirstOrDefaultAsync(x => x.UserName == "BrunoRM98");

            var gameRoomToJoin = await _db.GameRooms.FirstOrDefaultAsync(x => x.Id == data.Id);

            if (gameRoomToJoin is null)
                await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.GameRoomNotFound });

            var gameRoomPlayerIsAlready = (await _db.GameRoomPlayers
                .Include(x => x.GameRoom)
                .FirstOrDefaultAsync(x => x.PlayerId == player.Id)).GameRoom;

            if (gameRoomPlayerIsAlready != null && gameRoomPlayerIsAlready.Id != gameRoomToJoin.Id)
                await Clients.Caller.SendAsync("error", new { ok = false, alread_in_gameRoom = true, gameRoom = _mapper.Map<GameRoom, GameRoomDto>(gameRoomPlayerIsAlready) });

            if (!string.IsNullOrEmpty(gameRoomToJoin.Password))
                await ValidatePassword(data.Password, player, gameRoomToJoin);

            await ConnectToGameroom(player, gameRoomToJoin);
        }

        private async Task ValidatePassword(string password, Player player, GameRoom gameRoom)
        {
            if (string.IsNullOrEmpty(password))
                await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.InvalidPasswordRequired });

            var hashGenerator = new MD5HashGenerator();
            string passwordHash = hashGenerator.GetMD5Hash(password);

            if (hashGenerator.VerifyMd5Hash(passwordHash, gameRoom.Password))
                await ConnectToGameroom(player, gameRoom);
        }

        private async Task ConnectToGameroom(Player player, GameRoom gameRoom)
        {
            gameRoom.AddPlayer(player, true);

            _db.GameRooms.Update(gameRoom);
            await _db.SaveChangesAsync();

            string connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, gameRoom.Id.ToString());

            await Clients.Caller.SendAsync("connected", new { ok = true, gameRoom = _mapper.Map<GameRoom, GameRoomDto>(gameRoom) });
        }
    }

    public class ConnectToGameRoom
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
    }

    public class JoinToGameRoom
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
    }
}
