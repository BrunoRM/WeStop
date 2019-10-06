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
        private Round _round;
        private ICollection<Player> _players;

        public RoundScorer(IGameStorage gameStorage, IPlayerStorage playerStorage)
        {
            _gameStorage = gameStorage;
            _playerStorage = playerStorage;
            _playersPontuations = new List<RoundPontuations>();
        }

        public async Task ProcessRoundPontuationAsync(Round round)
        {
            if (round.Finished)
            {
                return;
            }
            
            _round = round;
            _players = await _playerStorage.GetPlayersInRoundAsync(round.GameId);

            var gameId = round.GameId;
            var roundNumber = round.Number;
            var roundAnswers = _players.GetAnswers(roundNumber).ToList();
            var validations = _players.GetValidations(roundNumber).ToList();
            var gameThemes = await _gameStorage.GetThemesAsync(round.GameId);

            foreach (var theme in gameThemes)
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
                            GiveFivePointsForEachPlayer(gameId, roundNumber, theme, playersThatRepliedAnswer);
                        }
                        else
                        {
                            GiveTenPointsForEachPlayer(gameId, roundNumber, theme, playersThatRepliedAnswer);
                        }
                    }
                    else
                    {
                        var playersThatRepliedAnswer = roundAnswers.GetPlayersIdsThatRepliedAnswer(answer);
                        GiveZeroPointsForEachPlayer(gameId, roundNumber, theme, playersThatRepliedAnswer);
                    }
                }

                var playersWithBlankAnswers = roundAnswers.GetPlayersIdsWithBlankAnswers();
                GiveZeroPointsForEachPlayer(gameId, roundNumber, theme, playersWithBlankAnswers);
            }

            await SavePlayersPontuations();
            round.Finish();
        }

        private async Task SavePlayersPontuations()
        {
            foreach (var playerPontuations in _playersPontuations)
            {
                var player = _players.FirstOrDefault(p => p.Id == playerPontuations.PlayerId);

                if (player != null)
                {
                    player.Pontuations.Add(playerPontuations);
                    await _playerStorage.EditAsync(player);
                }
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
