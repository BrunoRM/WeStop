using System;

namespace WeStop.Api.Classes
{
    public sealed class Player
    {
        public Player(Guid gameId, User user, bool isAdmin)
        {
            GameId = gameId;
            User = user;
            IsReady = false;
            IsAdmin = isAdmin;
        }

        public Guid GameId { get; set; }
        public User User { get; private set; }
        public Guid Id => User.Id;
        public string UserName => User.UserName;
        public bool IsAdmin { get; private set; }
        public bool IsReady { get; private set; }

        public void ChangeStatus(bool isReady)
        {
            IsReady = isReady;
        }
    }
}