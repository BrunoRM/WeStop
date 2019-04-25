namespace WeStop.Domain
{
    public class Player : Entity
    {
        public Player(string name, string userName, string password, string email)
        {
            Name = name;
            UserName = userName;
            Password = password;
            Email = email;
        }

        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
