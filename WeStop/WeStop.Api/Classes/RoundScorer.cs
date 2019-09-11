using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Extensions;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Classes
{
    public sealed class RoundScorer
    {
        private readonly IGameStorage _gameStorage;
        private readonly IAnswerStorage _answersStorage;
        private readonly IValidationStorage _validations;
        private readonly IPontuationStorage _gamePontuationStorage;
        private ICollection<RoundPontuations> _playersPontuations;

        public RoundScorer(IGameStorage gameStorage, IAnswerStorage answersStorage, IValidationStorage validationsStorage, IPontuationStorage gamePontuationStorage)
        {
            _gameStorage = gameStorage;
            _answersStorage = answersStorage;
            _validations = validationsStorage;
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
                    int validVotesCountForThemeAnswer = await _validations.GetValidVotesCountForAnswerAsync(gameId, roundNumber, answer);
                    int invalidVotesCountForThemeAnswer = await _validations.GetInvalidVotesCountForAnswerAsync(gameId, roundNumber, answer);

                    if (validVotesCountForThemeAnswer >= invalidVotesCountForThemeAnswer)
                    {
                        var playersThatRepliedAnswer = await _answersStorage.GetPlayersIdsThatRepliedAnswerAsync(gameId, roundNumber, answer);

                        if (!playersThatRepliedAnswer.Any())
                            continue;

                        if (playersThatRepliedAnswer.Count() > 1)
                            GiveFivePointsForEachPlayer(gameId, roundNumber, theme, playersThatRepliedAnswer);
                        else
                            GiveTenPointsForEachPlayer(gameId, roundNumber, theme, playersThatRepliedAnswer);
                    }
                    else
                    {
                        var playersThatRepliedAnswer = await _answersStorage.GetPlayersIdsThatRepliedAnswerAsync(gameId, roundNumber, answer);
                        GiveZeroPointsForEachPlayer(gameId, roundNumber, theme, playersThatRepliedAnswer);
                    }
                }

                var playersWithBlankAnswers = await _answersStorage.GetPlayersIdsWithBlankAnswersAsync(gameId, roundNumber, theme);
                GiveZeroPointsForEachPlayer(gameId, roundNumber, theme, playersWithBlankAnswers);
            }

            await SavePlayersPontuations();
        }

        private async Task SavePlayersPontuations()
        {
            foreach (var playerPontuations in _playersPontuations)
            {
                await _gamePontuationStorage.AddAsync(playerPontuations);
            }
        }

        private void GiveZeroPointsForEachPlayer(Guid gameId, int roundNumber, string theme, IEnumerable<Guid> playersIds)
        {
            foreach (var playerId in playersIds)
            {
                AddPlayerPontuation(gameId, roundNumber, playerId, theme, 0);
            }
        }

        private void GiveFivePointsForEachPlayer(Guid gameId, int roundNumber, string theme, IEnumerable<Guid> playersIds)
        {
            foreach (var playerId in playersIds)
            {
                AddPlayerPontuation(gameId, roundNumber, playerId, theme, 5);
            }
        }

        private void GiveTenPointsForEachPlayer(Guid gameId, int roundNumber, string theme, IEnumerable<Guid> playersIds)
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
