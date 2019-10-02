using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Extensions;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Domain.Services
{
    public sealed class RoundScorer
    {
        private readonly IGameStorage _gameStorage;
        private readonly IPlayerStorage _playerStorage;
        private readonly ICollection<RoundPontuations> _playersPontuations;
        private Game _game;

        public RoundScorer(IGameStorage gameStorage, IPlayerStorage playerStorage)
        {
            _gameStorage = gameStorage;
            _playerStorage = playerStorage;
            _playersPontuations = new List<RoundPontuations>();
        }

        public async Task<ICollection<RoundPontuations>> ProcessCurrentRoundPontuationAsync(Guid gameId)
        {
            _game = await _gameStorage.GetByIdAsync(gameId);
            var currentRoundNumber = _game.CurrentRoundNumber;
            var roundAnswers = _game.Players.GetAnswers(currentRoundNumber);
            var validations = _game.Players.GetValidations(currentRoundNumber);

            foreach (var theme in _game.Options.Themes)
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

            return _playersPontuations;
        }

        private async Task SavePlayersPontuations()
        {
            foreach (var playerPontuations in _playersPontuations)
            {
                var player = _game.Players.FirstOrDefault(p => p.Id == playerPontuations.PlayerId);
                player.Pontuations.Add(playerPontuations);
                await _playerStorage.EditAsync(player);
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
