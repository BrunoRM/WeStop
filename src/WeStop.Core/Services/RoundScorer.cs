﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Core.Extensions;
using WeStop.Core.Storages;

namespace WeStop.Core.Services
{
    public sealed class RoundScorer
    {
        private readonly IGameStorage _gameStorage;
        private readonly IPlayerStorage _playerStorage;

        public RoundScorer(IGameStorage gameStorage, IPlayerStorage playerStorage)
        {
            _gameStorage = gameStorage;
            _playerStorage = playerStorage;
        }

        public async Task ProcessRoundPontuationAsync(Round round)
        {
            if (round.Finished)
            {
                return;
            }

            var pontuations = new List<RoundPontuations>();
            var gameId = round.GameId;

            var players = await _playerStorage.GetPlayersInRoundAsync(gameId);

            var roundNumber = round.Number;
            var roundAnswers = players.GetAnswers(roundNumber).ToList();
            var validations = players.GetValidations(roundNumber).ToList();
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
                            GiveFivePointsForEachPlayer(theme, playersThatRepliedAnswer);
                        }
                        else
                        {
                            GiveTenPointsForEachPlayer(theme, playersThatRepliedAnswer);
                        }
                    }
                    else
                    {
                        var playersThatRepliedAnswer = roundAnswers.GetPlayersIdsThatRepliedAnswer(answer);
                        GiveZeroPointsForEachPlayer(theme, playersThatRepliedAnswer);
                    }
                }

                var playersWithBlankAnswers = roundAnswers.GetPlayersIdsWithBlankAnswersForTheme(theme);
                GiveZeroPointsForEachPlayer(theme, playersWithBlankAnswers);
            }
            
            await SavePlayersPontuations();
            round.Finish();

            #region Local Methods
            async Task SavePlayersPontuations()
            {
                foreach (var playerPontuations in pontuations)
                {
                    var player = players.FirstOrDefault(p => p.Id == playerPontuations.PlayerId);

                    if (player != null)
                    {
                        player.Pontuations.Add(playerPontuations);
                        await _playerStorage.EditAsync(player);
                    }
                }
            }

            void GiveZeroPointsForEachPlayer(string theme, IEnumerable<Guid> playersIds)
            {
                foreach (var playerId in playersIds)
                {
                    AddPlayerPontuation(playerId, theme, 0);
                }
            }

            void GiveFivePointsForEachPlayer(string theme, IEnumerable<Guid> playersIds)
            {
                foreach (var playerId in playersIds)
                {
                    AddPlayerPontuation(playerId, theme, 5);
                }
            }

            void GiveTenPointsForEachPlayer(string theme, IEnumerable<Guid> playersIds)
            {
                foreach (var playerId in playersIds)
                {
                    AddPlayerPontuation(playerId, theme, 10);
                }
            }

            void AddPlayerPontuation(Guid playerId, string theme, int pontuation)
            {
                var playerPontuations = pontuations.FirstOrDefault(pp => pp.PlayerId == playerId);

                if (playerPontuations is null)
                {
                    var roundPontuations = new RoundPontuations(gameId, roundNumber, playerId);
                    roundPontuations.AddPontuationForTheme(theme, pontuation);
                    pontuations.Add(roundPontuations);
                }
                else
                {
                    playerPontuations.AddPontuationForTheme(theme, pontuation);
                }
            }
            #endregion
        }
    }
}