using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Domain.Exceptions;

namespace WeStop.Domain
{
    public class GameRoom : Entity
    {
        private ICollection<Player> _players;

        public GameRoom(string name, string password, int numberOfRounds, int numberOfPlayers)
        {
            Name = name;
            Password = password;
            NumberOfRounds = numberOfRounds;
            NumberOfPlayers = numberOfPlayers;
            CreatedAt = DateTime.Now;
            Expiration = DateTime.Now.AddHours(1);
            Status = GameRoomStatus.Waiting;
            _players = new List<Player>();
        }

        public string Name { get; set; }
        public string Password { get; set; }
        public int NumberOfRounds { get; set; }
        public int NumberOfPlayers { get; set; }
        public GameRoomStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Expiration { get; set; }
        public IReadOnlyCollection<Player> Players { get { return _players.ToList(); } }

        public bool AddPlayer(Player player)
        {
            if (player is null) return false;

            if (Players.Count >= NumberOfPlayers)
                throw new GameRoomIsFullException();

            // Se o jogador já esta na sala, retorna uma resposta de sucesso
            if (_players.FirstOrDefault(x => x.Id == player.Id) != null)
                return true;

            _players.Add(player);

            return true;
        }
    }
}
