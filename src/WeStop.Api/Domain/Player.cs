using System;

namespace WeStop.Api.Domain
{
    public sealed class Player
    {
        private Player(Guid gameId, Guid userId, string userName, bool isAdmin)
        {
            GameId = gameId;
            Id = userId;
            UserName = userName;
            IsReady = false;
            IsAdmin = isAdmin;
            IsOnline = true;
            InRound = false;
        }

        public static Player CreateAsAdmin(Guid gameId, User user) =>
            new Player(gameId, user.Id, user.UserName, true);

        public static Player Create(Guid gameId, User user) =>
            new Player(gameId, user.Id, user.UserName, false);

        public Guid GameId { get; private set; }
        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public bool IsAdmin { get; private set; }
        public bool IsReady { get; set; }
        public bool IsOnline { get; private set; }
        public bool InRound { get; set; }
    }
}