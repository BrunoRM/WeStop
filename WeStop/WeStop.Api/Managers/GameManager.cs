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
        private readonly IAnswerStorage _answerStorage;
        private readonly IValidationStorage _validationStorage;
        private readonly IPontuationStorage _pontuationStorage;
        private readonly IUserStorage _userStorage;
        private readonly IPlayerStorage _playerStorage;

        public GameManager(IGameStorage gameStorage, IUserStorage userStorage,
            IAnswerStorage answerStorage, IValidationStorage validationStorage,
            IPontuationStorage pontuationStorage, IPlayerStorage playerStorage)
        {
            _gameStorage = gameStorage;
            _answerStorage = answerStorage;
            _validationStorage = validationStorage;
            _pontuationStorage = pontuationStorage;
            _userStorage = userStorage;
            _playerStorage = playerStorage;
        }

        public async Task<Game> CreateAsync(Guid userId, string name, string password, GameOptions options)
        {
            var user = await _userStorage.GetByIdAsync(userId);
            
            if (!string.IsNullOrEmpty(password))
            {
                // Criptografar a senha
            }

            var game = new Game(name, password, options);
            await _gameStorage.AddAsync(game);

            var player = Player.CreateAsAdmin(game.Id, user);
            await _playerStorage.AddAsync(player);

            return game;
        }

        public async Task<(Game Game, IReadOnlyCollection<Player> Players)> GetAsync(Guid gameId)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            var players = await _playerStorage.GetPlayersAsync(gameId);
            return (game, players.ToList());
        }

        public async Task JoinAsync(Guid gameId, Guid userId, Action<Game, Player> action)
        {
            Game game = await _gameStorage.GetByIdAsync(gameId);

            var player = await _playerStorage.GetAsync(gameId, userId);
            if (player is null)
            {
                var user = await _userStorage.GetByIdAsync(userId);
                player = Player.Create(gameId, user);
                await _playerStorage.AddAsync(player);
            }

            action?.Invoke(game, player);
        }

        public async Task StartNextRoundAsync(Guid gameId, Action<Game> action)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            game.StartNextRound();

            await _gameStorage.UpdateAsync(game);

            action?.Invoke(game);
        }

        public async Task AddCurrentRoundAnswersAsync(Guid gameId, Guid playerId, ICollection<Answer> answers)
        {
            var currentRoundNumber = await _gameStorage.GetCurrentRoundNumberAsync(gameId);
            var roundAnswers = new RoundAnswers(gameId, currentRoundNumber, playerId, answers);
            await _answerStorage.AddAsync(roundAnswers);
        }

        public async Task AddCurrentRoundValidationsAsync(Guid gameId, Guid playerId, ICollection<Validation> validations)
        {
            var currentRoundNumber = await _gameStorage.GetCurrentRoundNumberAsync(gameId);
            var roundValidations = new RoundValidations(gameId, currentRoundNumber, playerId, validations);
            await _validationStorage.AddAsync(roundValidations);
        }

        public async Task<ICollection<Validation>> GetCurrentRoundDefaultValidationsAsync(Guid gameId, int roundNumber, Guid playerId)
        {
            var playerValidations = await _validationStorage.GetValidationsAsync(gameId, roundNumber, playerId);

            if (!playerValidations.Any())
            {
                var answers = await _answerStorage.GetPlayersAnswersAsync(gameId, roundNumber);
                var defaultValidationsForPlayer = answers.BuildValidationsForPlayer(playerId);

                return defaultValidationsForPlayer.ToList();
            }

            return new List<Validation>();
        }

        public async Task<Guid[]> GetWinnersAsync(Guid gameId)
        {
            var playersPontuations = await _pontuationStorage.GetPontuationsAsync(gameId);
            var gameWinners = playersPontuations.GetWinners();
            return gameWinners;
        }

        public async Task StopCurrentRoundAsync(Guid gameId, Guid? playerId, Action<Game> action)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            // Alterar o estado da partida e salvar
            await _gameStorage.UpdateAsync(game);
            action?.Invoke(game);
        }

        public async Task FinishCurrentRoundAsync(Guid gameId, Action<ICollection<RoundPontuations>> action)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            game.FinishRound();

            await _gameStorage.UpdateAsync(game);

            var roundPontuation = await _pontuationStorage.GetPontuationsAsync(gameId, game.CurrentRound.Number);
            action?.Invoke(roundPontuation.ToList());
        }

        public async Task FinishAsync(Guid gameId)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            game.Finish();
            await _gameStorage.UpdateAsync(game);
        }
    }
}