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
        private readonly IPlayerStorage _playerStorage;

        public GameManager(IGameStorage gameStorage, IAnswerStorage answerStorage, 
            IValidationStorage validationStorage, IPontuationStorage pontuationStorage, 
            IPlayerStorage playerStorage)
        {
            _gameStorage = gameStorage;
            _answerStorage = answerStorage;
            _validationStorage = validationStorage;
            _pontuationStorage = pontuationStorage;
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

            var player = await _playerStorage.GetAsync(gameId, user.Id);
            if (player is null)
            {
                player = Player.Create(gameId, user);
                await _playerStorage.AddAsync(player);
                game.Players.Add(player);
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

        public async Task AddRoundAnswersAsync(RoundAnswers roundAnswers) =>
            await _answerStorage.AddAsync(roundAnswers);

        public async Task AddRoundValidationsAsync(RoundValidations roundValidations) =>
            await _validationStorage.AddAsync(roundValidations);

        public async Task<ICollection<Validation>> GetPlayerDefaultValidationsAsync(Guid gameId, int roundNumber, Guid playerId, string theme)
        {
            var playerValidations = await _validationStorage.GetValidationsAsync(gameId, roundNumber, playerId);

            if (!playerValidations.Any())
            {
                var answers = await _answerStorage.GetPlayersAnswersAsync(gameId, roundNumber);
                var defaultValidationsForPlayer = answers.BuildValidationsForPlayer(playerId, theme);

                return defaultValidationsForPlayer.ToList();
            }

            return new List<Validation>();
        }

        public async Task<ICollection<(Guid playerId, ICollection<Validation> validations)>> GetPlayersDefaultValidationsAsync(Guid gameId, string theme)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            var answers = await _answerStorage.GetPlayersAnswersAsync(gameId, game.CurrentRoundNumber);
            var players = await _playerStorage.GetPlayersAsync(gameId);

            var playersValidations = new List<(Guid PlayerId, ICollection<Validation> Validations)>();
            foreach (var player in players)
            {
                if (player.InRound)
                {
                    var playerValidations = answers.BuildValidationsForPlayer(player.Id, theme).ToList();
                    playersValidations.Add((player.Id, playerValidations));
                }
            }

            return playersValidations;
        }

        public async Task StartValidationForNextThemeAsync(Guid gameId, int roundNumber, Action<string> validationStartedAction, Action allThemesValidatedAction)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            foreach (var theme in game.Options.Themes)
            {
                if (!game.CurrentRound.ValidatedThemes.Contains(theme))
                {
                    game.CurrentRound.ThemeBeingValidated = theme;
                    validationStartedAction?.Invoke(theme);
                    return;
                }                
            }

            allThemesValidatedAction?.Invoke();
        }

        public async Task<Guid[]> GetWinnersAsync(Guid gameId)
        {
            var playersPontuations = await _pontuationStorage.GetPontuationsAsync(gameId);
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
            var validations = await _validationStorage.GetValidationsAsync(gameId, game.CurrentRoundNumber, theme);

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