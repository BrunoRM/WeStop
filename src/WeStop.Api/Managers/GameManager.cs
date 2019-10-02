using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Extensions;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Managers
{
    public class GameManager
    {
        private readonly IGameStorage _gameStorage;
        private readonly IPlayerStorage _playerStorage;

        public GameManager(IGameStorage gameStorage, IPlayerStorage playerStorage)
        {
            _gameStorage = gameStorage;
            _playerStorage = playerStorage;
        }

        public async Task<Game> CreateAsync(User user, string name, string password, GameOptions options)
        {            
            if (!string.IsNullOrEmpty(password))
            {
                // Criptografar a senha
            }

            var game = new Game(name, password, options);
            await _gameStorage.AddAsync(game);

            var player = Player.CreateAsAdmin(game.Id, user);
            await _playerStorage.AddAsync(player);
            game.Players.Add(player);
            
            return game;
        }

        public async Task JoinAsync(Guid gameId, User user, Action<Game, Player> action)
        {
            Game game = await _gameStorage.GetByIdAsync(gameId);

            var player = game.Players.FirstOrDefault(p => p.Id == user.Id);
            if (player is null)
            {
                player = Player.Create(gameId, user);
                game.Players.Add(player);
                await _playerStorage.AddAsync(player);
            }

            action?.Invoke(game, player);
        }

        public async Task StartRoundAsync(Guid gameId, Action<Round> action)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);

            var players = await _playerStorage.GetPlayersAsync(gameId);

            foreach (var player in players.PutReadyPlayersInRound())
            {
                await _playerStorage.UpdateAsync(player);
            }

            var createdRound = game.StartNextRound();
            await _gameStorage.UpdateAsync(game);

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

        public async Task<ICollection<Validation>> GetPlayerDefaultValidationsAsync(Guid gameId, int roundNumber, Guid playerId, string theme)
        {
            var players = await _playerStorage.GetPlayersInRoundAsync(gameId);
            var playerValidations = players.GetValidations(roundNumber);

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

        public async Task<string> StartValidationForNextThemeAsync(Guid gameId, int roundNumber)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            foreach (var theme in game.Options.Themes)
            {
                if (!game.CurrentRound.ValidatedThemes.Contains(theme))
                {
                    game.CurrentRound.ThemeBeingValidated = theme;
                    return theme;
                }                
            }

            return string.Empty;
        }

        public async Task<Guid[]> GetWinnersAsync(Guid gameId)
        {
            var players = await _playerStorage.GetPlayersInRoundAsync(gameId);
            var playersPontuations = players.GetPontuations();
            var gameWinners = playersPontuations.GetWinners();
            return gameWinners;
        }

        public async Task StopCurrentRoundAsync(Guid gameId, Action<Game> action)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            game.StartValidations();

            await _gameStorage.UpdateAsync(game);
            action?.Invoke(game);
        }

        public async Task FinishCurrentRoundAsync(Guid gameId)
        {
            var players = await _playerStorage.GetPlayersAsync(gameId);
            foreach (var player in players.PutAllPlayersInWaiting())
            {
                await _playerStorage.UpdateAsync(player);
            }

            var game = await _gameStorage.GetByIdAsync(gameId);
            game.FinishRound();
            await _gameStorage.UpdateAsync(game);
        }

        public async Task FinishAsync(Guid gameId)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            game.Finish();
            await _gameStorage.UpdateAsync(game);
        }

        public async Task<bool> AllPlayersSendValidationsAsync(Guid gameId, string theme)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            var validations = game.Players.GetValidations(game.CurrentRoundNumber);

            foreach (var player in game.Players)
            {
                if (player.InRound && !validations.Any(v => v.PlayerId == player.Id))
                    return false;
            }

            game.CurrentRound.ValidatedThemes.Add(theme);
            return true;
        }
    }
}