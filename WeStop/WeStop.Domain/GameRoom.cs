using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Domain
{
    public class GameRoom : Entity
    {
        private ICollection<Player> _players;

        public GameRoom(string name, bool isPrivate)
        {
            Name = name;
            IsPrivate = isPrivate;
            CreatedAt = DateTime.Now;
            Expiration = DateTime.Now.AddHours(1);
            Status = "Aguardando";
            _players = new List<Player>();
        }

        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Expiration { get; set; }
        public IReadOnlyCollection<Player> Players { get { return _players.ToList(); } }

        public bool AddPlayer(Player player)
        {
            if (player is null) return false;

            // Se o jogador já esta na sala, retorna uma resposta de sucesso
            if (_players.FirstOrDefault(x => x.Id == player.Id) != null)
                return true;

            _players.Add(player);

            return true;
        }
    }
}
