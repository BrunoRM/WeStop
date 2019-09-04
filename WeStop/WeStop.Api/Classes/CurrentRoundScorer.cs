using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void ProcessPontuationForGameCurrentRound(Game game)
        {
            string[] answersForTheme = GetPlayersAnswersForTheme(theme);

            ICollection<PlayerRound> _currentRoundPlayers = _currentRound.GetPlayers();
            foreach (var answer in answersForTheme)
            {
                int validVotesCountForThemeAnswer = _currentRoundPlayers.GetValidVotesCountForThemeAnswer(theme, answer);
                int invalidVotesCountForThemeAnswer = _currentRoundPlayers.GetInvalidVotesCountForThemeAnswer(theme, answer);

                if (validVotesCountForThemeAnswer >= invalidVotesCountForThemeAnswer)
                {
                    var players = GetPlayersThatRepliedAnswerForTheme(answer, theme);

                    if (!players.Any())
                    {
                        continue;
                    }

                    if (players.Count() > 1)
                    {
                        GenerateFiveThemePointsForEachPlayer(theme, players);
                    }
                    else
                    {
                        GenerateTenThemePointsForEachPlayer(theme, players);
                    }
                }
                else
                {
                    var players = GetPlayersThatRepliedAnswerForTheme(answer, theme);
                    GenerateZeroThemePointsForEachPlayer(theme, players);
                }
            }

            var playersWithBlankThemeAnswer = GetPlayersThatNotRepliedAnswerForTheme(theme);
            GenerateZeroThemePointsForEachPlayer(theme, playersWithBlankThemeAnswer);
        }

        private string[] GetPlayersAnswersForTheme(string theme)
        {
            return game.GetPlayers()
                .SelectMany(p => p.Answers.Where(a => a.Theme == theme && !string.IsNullOrEmpty(a.Answer)).Select(a => a.Answer)).Distinct().ToArray();
        }
    }
}
