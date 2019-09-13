using System;

namespace WeStop.Api.Domain
{
    public sealed class Player
    {
        public Player(Guid gameId, User user, bool isAdmin)
        {
            GameId = gameId;
            User = user;
            IsReady = false;
            IsAdmin = isAdmin;
            IsOnline = true;
            IsInRound = false;
        }

        public Guid GameId { get; set; }
        public User User { get; private set; }
        public Guid Id => User.Id;
        public string UserName => User.UserName;
        public bool IsAdmin { get; private set; }
        public bool IsReady { get; private set; }
        public bool IsOnline { get; set; }
        public bool IsInRound { get; set; }

        public void ChangeStatus(bool isReady)
        {
            IsReady = isReady;
        }
    }
}