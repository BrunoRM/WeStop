using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Core.Extensions
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
                yield return player;
            }
        }

        public static IEnumerable<RoundAnswers> GetAnswers(this ICollection<Player> players, int roundNumber) =>
            players.SelectMany(p => p.Answers.Where(v => v.RoundNumber == roundNumber));

        public static IEnumerable<RoundValidations> GetValidations(this ICollection<Player> players, int roundNumber) =>
            players.SelectMany(p => p.Validations.Where(v => v.RoundNumber == roundNumber));

        public static IEnumerable<RoundValidations> GetValidations(this ICollection<Player> players, int roundNumber, string theme) =>
            players.SelectMany(p => p.Validations.Where(v => v.RoundNumber == roundNumber && v.Theme.Equals(theme)));

        public static int GetHighPontuation(this ICollection<Player> players) =>
            players.Max(p => p.TotalPontuation);

        public static IReadOnlyCollection<Player> GetPlayersWithPontuation(this ICollection<Player> players, int pontuation) =>
            players.Where(p => p.TotalPontuation == pontuation).ToList();

        public static IEnumerable<RoundPontuations> GetInRoundPontuations(this ICollection<Player> players, int roundNumber) =>
            players.SelectMany(p => p.Pontuations.Where(pt => pt.RoundNumber == roundNumber));

        public static bool IsValidationsRequiredForPlayer(this ICollection<Player> players, Guid playerId, int roundNumber, string theme)
        {
            return players.Where(p => p.Id != playerId).Any(p => p.Answers.Any(ra => ra.RoundNumber == roundNumber && 
                ra.Answers.Any(a => a.Theme.Equals(theme) && !a.IsEmpty())));
        }

        public static bool HasAnyAnswerForTheme(this ICollection<Player> players, int roundNumber, string theme) =>
            players.Any(p => p.Answers.Where(ra => ra.RoundNumber == roundNumber).Any(ra => ra.Answers.Any(a => a.Theme.Equals(theme) && !a.IsEmpty())));

        public static Player GetOldestPlayerInGame(this ICollection<Player> players) =>
            players.OrderBy(x => x.JoinedDate).First();
    }
}
