using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Domain.Services;
using WeStop.Api.Extensions;
using WeStop.Api.Helpers;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Managers
{
    public class GameManager
    {
        private readonly IGameStorage _gameStorage;
        private readonly IPlayerStorage _playerStorage;
        private readonly RoundScorer _roundScorer;

        public GameManager(IGameStorage gameStorage, IPlayerStorage playerStorage,
            RoundScorer roundScorer)
        {
            _gameStorage = gameStorage;
            _playerStorage = playerStorage;
            _roundScorer = roundScorer;
        }

        public async Task<Game> CreateAsync(User user, string name, string password, GameOptions options)
        {
            // TODO: mover essa lógica de criptografia pra dentro da classe Game
            if (!string.IsNullOrEmpty(password))
            {
                password = MD5HashGenerator.GenerateHash(password);
            }
            ///

            var game = new Game(name, password, options);
            await _gameStorage.AddAsync(game);

            var player = Player.CreateAsAdmin(game.Id, user);
            await _playerStorage.AddAsync(player);
            game.Players.Add(player);
            
            return game;
        }

        public async Task JoinAsync(Guid gameId, string password, User user, Action<Game, Player> successAction, Action<string> failureAction)
        {
            Game game = await _gameStorage.GetByIdAsync(gameId);

            if (game.IsPrivate())
            {
                if (string.IsNullOrEmpty(password))
                {
                    failureAction?.Invoke("PASSWORD_REQUIRED");
                    return;
                }
                else if (!MD5HashGenerator.GenerateHash(password).Equals(game.Password))
                {
                    failureAction?.Invoke("PASSWORD_INCORRECT");
                    return;
                }
            }

            var player = game.Players.FirstOrDefault(p => p.Id == user.Id);
            if (player is null)
            {
                player = Player.Create(gameId, user);
                game.Players.Add(player);
                await _playerStorage.AddAsync(player);
            }

            successAction?.Invoke(game, player);
        }

        public async Task StartRoundAsync(Guid gameId, Action<Round> action)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);

            foreach (var player in game.Players.PutReadyPlayersInRound())
            {
                await _playerStorage.EditAsync(player);
            }

            var createdRound = game.StartNextRound();
            await _gameStorage.EditAsync(game);

            action?.Invoke(createdRound);
        }

        public async Task AddRoundAnswersAsync(RoundAnswers roundAnswers)
        {
            var player = await _playerStorage.GetAsync(roundAnswers.GameId, roundAnswers.PlayerId);
            player.Answers.Add(roundAnswers);
            await _playerStorage.EditAsync(player);
        }

        public async Task AddRoundValidationsAsync(RoundValidations roundValidations)
        {
            var player = await _playerStorage.GetAsync(roundValidations.GameId, roundValidations.PlayerId);
            player.Validations.Add(roundValidations);
            await _playerStorage.EditAsync(player);
        }

        public async Task<ICollection<Guid>> GetInRoundPlayersIdsAsync(Guid gameId)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);

            var ids = new List<Guid>();
            foreach (var player in game.Players)
            {
                if (player.InRound)
                {
                    ids.Add(player.Id);
                }
            }

            return ids;
        }

        public async Task<ICollection<Validation>> GetPlayerDefaultValidationsAsync(Guid gameId, int roundNumber, Guid playerId, string theme)
        {
            var players = await _playerStorage.GetPlayersInRoundAsync(gameId);
            var playerValidations = players.GetValidations(roundNumber, theme);

            if (!playerValidations.Any())
            {
                var answers = players.GetAnswers(roundNumber);
                var defaultValidationsForPlayer = answers.BuildValidationsForPlayer(playerId, theme);

                return defaultValidationsForPlayer.ToList();
            }

            return new List<Validation>();
        }

        public async Task<ICollection<(Guid playerId, ICollection<Validation> validations)>> GetPlayersDefaultValidationsAsync(Guid gameId, string theme)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            var answers = game.Players.GetAnswers(game.CurrentRoundNumber);

            var playersValidations = new List<(Guid PlayerId, ICollection<Validation> Validations)>();
            foreach (var player in game.Players)
            {
                if (player.InRound)
                {
                    var playerValidations = answers.BuildValidationsForPlayer(player.Id, theme).ToList();
                    playersValidations.Add((player.Id, playerValidations));
                }
            }

            return playersValidations;
        }

        public async Task<bool> AllPlayersSendValidationsAsync(Guid gameId, string theme)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            var validations = game.Players.GetValidations(game.CurrentRoundNumber);

            foreach (var player in game.Players)
            {
                if (player.InRound && !validations.Any(v => v.PlayerId == player.Id && v.Theme.Equals(theme)))
                    return false;
            }

            game.CurrentRound.ValidatedThemes.Add(theme);
            await _gameStorage.EditAsync(game);

            return true;
        }

        public async Task<string> StartValidationForNextThemeAsync(Guid gameId)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            foreach (var theme in game.Options.Themes)
            {
                if (!game.CurrentRound.ValidatedThemes.Contains(theme))
                {
                    game.StartValidations();
                    game.CurrentRound.ThemeBeingValidated = theme;
                    await _gameStorage.EditAsync(game);
                    return theme;
                }                
            }

            return string.Empty;
        }

        public async Task StopCurrentRoundAsync(Guid gameId, Action<Game> action)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            game.StartValidations();

            await _gameStorage.EditAsync(game);
            action?.Invoke(game);
        }

        public async Task FinishCurrentRoundAsync(Guid gameId, Action<bool, IReadOnlyCollection<PlayerPontuation>, IEnumerable<string>> finishedRoundAction)
        {
            var players = await _playerStorage.GetPlayersAsync(gameId);

            var game = await _gameStorage.GetByIdAsync(gameId);
            game.FinishRound(); // TODO: Revisar, pois Round possui um método finish também

            await _roundScorer.ProcessRoundPontuationAsync(game.CurrentRound);

            var roundScoreboard = game.GetScoreboard(game.CurrentRoundNumber);
            if (game.IsFinalRound())
            {
                game.Finish();
                var winners = game.GetWinners().ToList();
                finishedRoundAction?.Invoke(true, roundScoreboard, winners);
            }
            else
            {
                finishedRoundAction?.Invoke(false, roundScoreboard, new List<string>());
            }

            foreach (var player in players.PutAllPlayersInWaiting())
            {
                await _playerStorage.EditAsync(player);
            }

            await _gameStorage.EditAsync(game);
        }

        public async Task FinishAsync(Guid gameId)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            game.Finish();
            await _gameStorage.EditAsync(game);
        }
    }
}