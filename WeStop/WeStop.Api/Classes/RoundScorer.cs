using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Classes
{
    public sealed class RoundScorer
    {
        private readonly IGameStorage _gameStorage;
        private readonly IPlayerRoundAnswersStorage _answersStorage;
        private readonly IPlayerRoundValidationsStorage _validationsStorage;
        private readonly IGamePontuationStorage _gamePontuationStorage;
        private ICollection<RoundPontuations> _playersPontuations;

        public RoundScorer(IGameStorage gameStorage, IPlayerRoundAnswersStorage answersStorage, IPlayerRoundValidationsStorage validationsStorage, IGamePontuationStorage gamePontuationStorage)
        {
            _gameStorage = gameStorage;
            _answersStorage = answersStorage;
            _validationsStorage = validationsStorage;
            _gamePontuationStorage = gamePontuationStorage;
        }

        public async Task ProcessRoundPontuationAsync(Guid gameId, int roundNumber)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            foreach (var theme in game.Options.Themes)
            {
                var answersForTheme = await _answersStorage.GetDistinctsAnswersForThemeAsync(gameId, roundNumber, theme);

                foreach (var answer in answersForTheme)
                {
                    int validVotesCountForThemeAnswer = await _validationsStorage.GetValidVotesCountForThemeAnswerAsync(gameId, roundNumber, theme, answer);
                    int invalidVotesCountForThemeAnswer = await _validationsStorage.GetInvalidVotesCountForThemeAnswerAsync(gameId, roundNumber, theme, answer);

                    if (validVotesCountForThemeAnswer >= invalidVotesCountForThemeAnswer)
                    {
                        var playersThatRepliedAnswerForTheme = await _answersStorage.GetPlayersIdsThatRepliedAnswerForThemeInRoundAsync(gameId, roundNumber, theme, answer);

                        if (!playersThatRepliedAnswerForTheme.Any())
                            continue;

                        if (playersThatRepliedAnswerForTheme.Count() > 1)
                            GiveFiveThemePointsForEachPlayerInRound(gameId, roundNumber, theme, playersThatRepliedAnswerForTheme);
                        else
                            GiveTenThemePointsForEachPlayerInRound(gameId, roundNumber, theme, playersThatRepliedAnswerForTheme);
                    }
                    else
                    {
                        var playersThatRepliedAnswerForTheme = await _answersStorage.GetPlayersIdsThatRepliedAnswerForThemeInRoundAsync(gameId, roundNumber, theme, answer);
                        GiveZeroThemePointsForEachPlayerInRound(gameId, roundNumber, theme, playersThatRepliedAnswerForTheme);
                    }
                }

                var playersWithBlankAnswers = await _answersStorage.GetPlayersIdsWithBlankAnswersForThemeInRoundAsync(gameId, roundNumber, theme);
                GiveZeroThemePointsForEachPlayerInRound(gameId, roundNumber, theme, playersWithBlankAnswers);
            }

            foreach (var playerPontuations in _playersPontuations)
            {
                await _gamePontuationStorage.AddAsync(playerPontuations);
            }
        }

        private void GiveZeroThemePointsForEachPlayerInRound(Guid gameId, int roundNumber, string theme, IEnumerable<Guid> playersIds)
        {
            foreach (var playerId in playersIds)
            {
                AddPlayerPontuation(gameId, roundNumber, playerId, theme, 0);
            }
        }

        private void GiveFiveThemePointsForEachPlayerInRound(Guid gameId, int roundNumber, string theme, IEnumerable<Guid> playersIds)
        {
            foreach (var playerId in playersIds)
            {
                AddPlayerPontuation(gameId, roundNumber, playerId, theme, 5);
            }
        }

        private void GiveTenThemePointsForEachPlayerInRound(Guid gameId, int roundNumber, string theme, IEnumerable<Guid> playersIds)
        {
            foreach (var playerId in playersIds)
            {
                AddPlayerPontuation(gameId, roundNumber, playerId, theme, 10);
            }
        }

        private void AddPlayerPontuation(Guid gameId, int roundNumber, Guid playerId, string theme, int pontuation)
        {
            var playerPontuations = _playersPontuations.FirstOrDefault(pp => pp.PlayerId == playerId);

            if (playerPontuations is null)
            {
                var roundPontuations = new RoundPontuations(gameId, roundNumber, playerId, new ThemePontuation(theme, pontuation));
                _playersPontuations.Add(roundPontuations);
            }
            else
            {
                playerPontuations.AddPontuationForTheme(theme, pontuation);
            }
        }
    }
}
