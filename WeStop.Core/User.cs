using System;

namespace WeStop.Core
{
    public class User
    {
        public User(Guid id, string userName, string imageUri)
        {
            Id = id;
            UserName = userName;
            ImageUri = imageUri;
        }
        
        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string ImageUri { get; private set; }
    }
}