using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Exceptions;

namespace WeStop.Api.Classes
{
    public sealed class GameScoreboardService
    {
        public GameScoreboardService(Game game)
        {
            if (game is null)
                throw new ArgumentNullException("game", "Parâmetro deve ser informado");

            this.Game = game;
        }
        
        public Game Game { get; private set; }

        public ICollection<RoundScoreboard> GetRoundScoreboard(Guid roundId)
        {
            var round = Game.Rounds.Where(r => r.Id == roundId);

            if (round is null)
                throw new WeStopException("Round não encontrado");

            var players = Game.Players;

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