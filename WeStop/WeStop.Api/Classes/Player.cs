using System;

namespace WeStop.Api.Classes
{
    public class Player
    {
        public Player(User user, bool isAdmin)
        {
            User = user;
            IsReady = false;
            IsAdmin = false;
            EarnedPoints = 0;
            IsAdmin = isAdmin;
            Status = PlayerStatus.Online;
        }

        public User User { get; private set; }
        public Guid Id => User.Id;
        public string UserName => User.UserName;
        public bool IsAdmin { get; private set; }
        public bool IsReady { get; private set; }
        public int EarnedPoints { get; private set; }
        public PlayerStatus Status { get; private set; }

        public void AddPoints(int points)
        {
            if (points >= 0 && points <= 10)
                EarnedPoints += points;
        }

        public void ChangeStatus(bool isReady)
        {
            IsReady = isReady;
        }

        public void SetOnline() =>
            Status = PlayerStatus.Online;

        public void SetOffline() =>
            Status = PlayerStatus.Offline;
    }
}