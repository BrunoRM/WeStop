using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public class Round
    {
        public Round()
        {
            Finished = false;
            Players = new List<PlayerRound>();
        }

        public int Number { get; set; }
        public string SortedLetter { get; set; }
        public bool Finished { get; set; }
        public ICollection<PlayerRound> Players { get; set; }

        public ICollection<PlayerRound> GetPlayersOnline() =>
            Players.Where(pr => pr.Player.Status == PlayerStatus.Online).ToList();
    }
}