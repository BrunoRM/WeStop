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

        public GameManager(IGameStorage gameStorage, IUserStorage userStorage,
            IAnswerStorage answerStorage, IValidationStorage validationStorage,
            IPontuationStorage pontuationStorage)
        {
            _gameStorage = gameStorage;
            _answerStorage = answerStorage;
            _validationStorage = validationStorage;
            _pontuationStorage = pontuationStorage;
            _userStorage = userStorage;
        }

        public async Task<Game> CreateAsync(Guid userId, string name, string password, GameOptions options)
        {
            var user = await _userStorage.GetByIdAsync(userId);
            
            if (!string.IsNullOrEmpty(password))
            {
                // Criptografar a senha
            }

            var game = new Game(user, name, password, options);
            await _gameStorage.AddAsync(game);

            return game;
        }

        public async Task<Game> Get(Guid gameId)
        {
            return await _gameStorage.GetByIdAsync(gameId);
        }

        public async Task JoinToGameAsync(Guid gameId, Guid userId, Action<Game, Player> action)
        {
            Game game = await _gameStorage.GetByIdAsync(gameId);

            if (!game.HasPlayerInGame(userId))
            {
                var user = await _userStorage.GetByIdAsync(userId);
                game.AddPlayer(user);
            }

            var player = game.GetPlayer(userId);
            action?.Invoke(game, player);
        }

        public async Task StartNextRoundAsync(Guid gameId, Action<Game> action)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            game.StartNextRound();

            await _gameStorage.UpdateAsync(game);

            action?.Invoke(game);
        }

        public async Task<ICollection<Validation>> GetDefaultPlayerValidationsAsync(Guid gameId, int roundNumber, Guid playerId)
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
    }
}