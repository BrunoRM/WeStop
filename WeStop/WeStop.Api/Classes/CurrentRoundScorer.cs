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
                            GiveFiveThemePointsForEachPlayer(theme, players);
                        else
                            GiveTenThemePointsForEachPlayer(theme, players);
                    }
                    else
                    {
                        var playersThatRepliedAnswerForTheme = players.GetPlayersThatRepliedAnswerForThemeInRound(gameCurrentRoundId, answer, theme);
                        GiveZeroThemePointsForEachPlayer(theme, players);
                    }
                }

                var playersWithBlankThemeAnswer = players.GetPlayersThatNotRepliedAnswerForThemeInRound(gameCurrentRoundId, theme);
                GiveZeroThemePointsForEachPlayer(theme, playersWithBlankThemeAnswer);
            }
        }

        private void GiveZeroThemePointsForEachPlayer(string theme, IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                // Método de pontos no player precisa receber pontuação por tema
            }
        }

        private void GiveFiveThemePointsForEachPlayer(string theme, IEnumerable<Player> players)
        {
            throw new NotImplementedException();
        }

        private void GiveTenThemePointsForEachPlayer(string theme, IEnumerable<Player> players)
        {
            throw new NotImplementedException();
        }
    }
}
