using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Extensions;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Managers
{
    public sealed class RoundScorerManager
    {
        private readonly IGameStorage _gameStorage;
        private readonly IAnswerStorage _answersStorage;
        private readonly IValidationStorage _validations;
        private readonly IPontuationStorage _gamePontuationStorage;
        private readonly ICollection<RoundPontuations> _playersPontuations;

        public RoundScorerManager(IGameStorage gameStorage, IAnswerStorage answersStorage, 
            IValidationStorage validationsStorage, IPontuationStorage gamePontuationStorage)
        {
            _gameStorage = gameStorage;
            _answersStorage = answersStorage;
            _validations = validationsStorage;
            _gamePontuationStorage = gamePontuationStorage;
            _playersPontuations = new List<RoundPontuations>();
        }

        public async Task ProcessCurrentRoundPontuationAsync(Guid gameId, Action<ICollection<RoundPontuations>> action)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            var currentRoundNumber = game.CurrentRound.Number;
            var validations = await _validations.GetValidationsAsync(gameId, currentRoundNumber);
            var roundAnswers = await _answersStorage.GetPlayersAnswersAsync(gameId, currentRoundNumber);

            foreach (var theme in game.Options.Themes)
            {
                var answers = roundAnswers.GetAnswersOfTheme(theme);

                foreach (var answer in answers)
                {
                    int validVotesCountForAnswer = validations.GetValidVotesCountForAnswer(answer);
                    int invalidVotesCountForAnswer = validations.GetInvalidVotesCountForAnswer(answer);

                    if (validVotesCountForAnswer >= invalidVotesCountForAnswer)
                    {
                        var playersThatRepliedAnswer = roundAnswers.GetPlayersIdsThatRepliedAnswer(answer);

                        if (!playersThatRepliedAnswer.Any())
                        {
                            continue;
                        }

                        if (playersThatRepliedAnswer.Count() > 1)
                        {
                            GiveFivePointsForEachPlayer(gameId, currentRoundNumber, theme, playersThatRepliedAnswer);
                        }
                        else
                        {
                            GiveTenPointsForEachPlayer(gameId, currentRoundNumber, theme, playersThatRepliedAnswer);
                        }
                    }
                    else
                    {
                        var playersThatRepliedAnswer = roundAnswers.GetPlayersIdsThatRepliedAnswer(answer);
                        GiveZeroPointsForEachPlayer(gameId, currentRoundNumber, theme, playersThatRepliedAnswer);
                    }
                }

                var playersWithBlankAnswers = roundAnswers.GetPlayersIdsWithBlankAnswers();
                GiveZeroPointsForEachPlayer(gameId, currentRoundNumber, theme, playersWithBlankAnswers);
            }

            await SavePlayersPontuations();
            action?.Invoke(_playersPontuations);
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
