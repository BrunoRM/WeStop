using System;

namespace WeStop.Api.Domain
{
    public sealed class Player
    {
        private Player(Guid gameId, User user, bool isAdmin)
        {
            GameId = gameId;
            User = user;
            IsReady = false;
            IsAdmin = isAdmin;
            IsOnline = true;
            InRound = false;
        }

        public static Player CreateAsAdmin(Guid gameId, User user) =>
            new Player(gameId, user, true);

        public static Player Create(Guid gameId, User user) =>
            new Player(gameId, user, false);

        public Guid GameId { get; private set; }
        public User User { get; set; }
        public Guid Id => User.Id;
        public string UserName => User.UserName;
        public bool IsAdmin { get; private set; }
        public bool IsReady { get; set; }
        public bool IsOnline { get; private set; }
        public bool InRound { get; set; }
    }
}