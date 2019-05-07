using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Domain.Exceptions;

namespace WeStop.Domain
{
    public class GameRoom : Entity
    {
        private List<GameRoomPlayer> _players;

        public GameRoom(string name, string password, int numberOfRounds, int numberOfPlayers, string themes, string availableLetters)
        {
            Name = name;
            Password = password;
            NumberOfRounds = numberOfRounds;
            NumberOfPlayers = numberOfPlayers;
            Themes = themes;
            AvailableLetters = availableLetters;
            CreatedAt = DateTime.Now;
            Expiration = DateTime.Now.AddHours(1);
            Status = GameRoomStatus.Waiting;
            _players = new List<GameRoomPlayer>();
        }

        public string Name { get; set; }
        public string Password { get; set; }
        public int NumberOfRounds { get; set; }
        public int NumberOfPlayers { get; set; }
        public string Themes { get; set; }
        public string AvailableLetters { get; set; }
        public GameRoomStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Expiration { get; set; }
        public IReadOnlyCollection<GameRoomPlayer> Players => _players.AsReadOnly();

        public bool AddPlayer(Player player, bool isAdmin)
        {
            if (player is null) return false;

            if (Players.Count >= NumberOfPlayers)
                throw new GameRoomIsFullException();

            // Se o jogador já esta na sala, retorna uma resposta de sucesso
            if (_players.FirstOrDefault(x => x.PlayerId == player.Id) != null)
                return true;

            _players.Add(new GameRoomPlayer(Id, player.Id, isAdmin));

            return true;
        }
    }
}
