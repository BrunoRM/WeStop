using System;

namespace WeStop.Api.Classes
{
    public class User
    {
        public User(string userName)
        {
            Id = Guid.NewGuid();
            UserName = userName;
        }

        public Guid Id { get; private set; }
        public string UserName { get; private set; }
    }
}