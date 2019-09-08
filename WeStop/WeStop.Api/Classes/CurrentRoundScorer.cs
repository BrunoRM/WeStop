using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Extensions;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Classes
{
    public sealed class CurrentRoundScorer
    {
        private readonly IGameStorage _gameStorage;

        public CurrentRoundScorer(IGameStorage gameStorage)
        {
            _gameStorage = gameStorage;
        }

        public async Task ProcessPontuationForGameCurrentRound(Guid gameId)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);

            var gameCurrentRoundId = game.CurrentRound.Id;
            foreach (var theme in game.Options.Themes)
            {
                var players = game.Players;
                var answersForTheme = players.GetRoundDistinctsAnswersByTheme(gameCurrentRoundId, theme);

                foreach (var answer in answersForTheme)
                {
                    var validVotesCountForThemeAnswer = players.GetRoundValidVotesCountForThemeAnswer(gameCurrentRoundId, theme, answer);
                    var invalidVotesCountForThemeAnswer = players.GetRoundInvalidVotesCountForThemeAnswer(gameCurrentRoundId, theme, answer);

                    if (validVotesCountForThemeAnswer >= invalidVotesCountForThemeAnswer)
                    {
                        var playersThatRepliedAnswerForTheme = players.GetPlayersThatRepliedAnswerForThemeInRound(gameCurrentRoundId, answer, theme);

                        if (!playersThatRepliedAnswerForTheme.Any())
                            continue;

                        if (playersThatRepliedAnswerForTheme.Count() > 1)
                            GiveFiveThemePointsForEachPlayerInRound(gameCurrentRoundId, theme, players);
                        else
                            GiveTenThemePointsForEachPlayerInRound(gameCurrentRoundId,theme, players);
                    }
                    else
                    {
                        var playersThatRepliedAnswerForTheme = players.GetPlayersThatRepliedAnswerForThemeInRound(gameCurrentRoundId, answer, theme);
                        GiveZeroThemePointsForEachPlayerInRound(gameCurrentRoundId,theme, players);
                    }
                }

                var playersWithBlankThemeAnswer = players.GetPlayersThatNotRepliedAnswerForThemeInRound(gameCurrentRoundId, theme);
                GiveZeroThemePointsForEachPlayerInRound(gameCurrentRoundId,theme, playersWithBlankThemeAnswer);
            }
        }

        private void GiveZeroThemePointsForEachPlayerInRound(Guid roundId, string theme, IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                player.AddPontuationForThemeInRound(roundId, theme, 0);
            }
        }

        private void GiveFiveThemePointsForEachPlayerInRound(Guid roundId,string theme, IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                player.AddPontuationForThemeInRound(roundId, theme, 5);
            }
        }

        private void GiveTenThemePointsForEachPlayerInRound(Guid roundId,string theme, IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                player.AddPontuationForThemeInRound(roundId, theme, 10);
            }
        }
    }
}
