using System;

namespace WeStop.Api.Core
{
    public class User
    {
        public User(Guid id, string userName)
        {
            this.Id = id;
            this.UserName = userName;
        }
        
        public Guid Id { get; private set; }
        public string UserName { get; set; }
    }
}