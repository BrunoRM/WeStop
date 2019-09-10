using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Exceptions;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Classes
{
    public sealed class GameScoreboardService
    {
        private readonly IGamePontuationStorage _gamePontuation;

        public GameScoreboardService(IGamePontuationStorage gamePontuationStorage)
        {
            this._gamePontuation = gamePontuationStorage;
        }

        public async Task<ICollection<RoundScoreboard>> GetRoundScoreboardAsync(Guid gameId, int roundNumber)
        {
            var roundPontuations = await _gamePontuation.GetPlayersPontuationsForRoundAsync(gameId, roundNumber);

            return roundPontuations.Select(rp => new RoundScoreboard
            {
                PlayerId = rp.PlayerId,
                UserName = "",
                Pontuation = rp.
            })

            return players.Select(p => new RoundScoreboard(p.Id, p.UserName, p[roundId]))
                .OrderByDescending(rs => rs.Pontuation).ToList();
        }

        public ICollection<GameScoreboard> GetScoreboard()
        {
            var players = Game.Players;

            return players.Select(p => new GameScoreboard(p.Id, p.UserName, p.EarnedPoints))
                .OrderByDescending(ps => ps.Pontuation).ToList();
        }

        public string[] GetWinners()
        {
            if (!Game.IsFinalRound())
                throw new WeStopException("Só é possível eleger um vencedor se a partida já foi finalizada");
            
            var players = Game.Players;

            int playersBestPontuation = players.Max(p => p.EarnedPoints);

            return players.Where(p => p.EarnedPoints == playersBestPontuation)
                .OrderBy(p => p.UserName)
                .Select(p => p.UserName).ToArray();
        }
    }
}