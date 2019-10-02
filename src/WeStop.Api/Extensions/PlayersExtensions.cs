using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Domain;

namespace WeStop.Api.Extensions
{
    public static class PlayersExtensions
    {
        public static IEnumerable<Player> PutReadyPlayersInRound(this ICollection<Player> players)
        {
            foreach (var player in players)
            {
                if (player.IsReady)
                {
                    player.InRound = true;
                    yield return player;
                }
            }
        }

        public static IEnumerable<Player> PutAllPlayersInWaiting(this ICollection<Player> players)
        {
            foreach (var player in players)
            {
                player.InRound = false;
                player.IsReady = false;
                yield return player;
            }
        }
        public static IEnumerable<RoundAnswers> GetAnswers(this ICollection<Player> players, int roundNumber) =>
            players.SelectMany(p => p.Answers.Where(v => v.RoundNumber == roundNumber));

        public static IEnumerable<RoundValidations> GetValidations(this ICollection<Player> players, int roundNumber) =>
            players.SelectMany(p => p.Validations.Where(v => v.RoundNumber == roundNumber));

        public static IEnumerable<RoundPontuations> GetPontuations(this ICollection<Player> players) =>
            players.SelectMany(p => p.Pontuations);

        public static IEnumerable<RoundPontuations> GetInRoundPontuations(this ICollection<Player> players, int roundNumber) =>
            players.SelectMany(p => p.Pontuations.Where(pt => pt.RoundNumber == roundNumber));
    }
}
