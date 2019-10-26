using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Core.Extensions;
using WeStop.Core.Helpers;
using WeStop.Core.Storages;

namespace WeStop.Core.Services
{
    /// Observa��o sobre a decis�o de design desta classe:
    /// Os m�todos que envolvem envio de valida��es e verifica��o para validar se todos os players j� enviaram
    /// suas valida��es precisam ser sincronos, para o servi�o poder enviar uma por vez aos storages, evitando
    /// a concorr�ncia, que levaria a dados inconsistentes.
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
            if (!string.IsNullOrEmpty(password))
            {
                password = MD5HashGenerator.GenerateHash(password);
            }

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

        public async Task<(ICollection<Validation> Validations, int TotalValidations, int ValidationsNumber)> GetPlayerDefaultValidationsAsync(Guid gameId, int roundNumber, Guid playerId, string theme, string sortedLetter)
        {
            var players = await _playerStorage.GetPlayersInRoundAsync(gameId);
            var playerValidations = players.GetValidations(roundNumber, theme);

            if (!playerValidations.HasValidatiosOfPlayer(playerId))
            {
                var answers = players.GetAnswers(roundNumber);
                var totalValidations = answers.GetTotalThemesForPlayerValidate(playerId, roundNumber);
                var defaultValidationsForPlayer = answers.BuildValidationsForPlayer(playerId, theme, sortedLetter);

                var validationsNumber = players.First(p => p.Id == playerId).GetTotalValidationsInRound(roundNumber) + 1;
                return (defaultValidationsForPlayer.ToList(), totalValidations, validationsNumber);
            }

            return (new List<Validation>(), 0, 0);
        }

        public async Task<ICollection<(Guid PlayerId, ICollection<Validation> Validations, int TotalValidations, int ValidationsNumber)>> GetPlayersDefaultValidationsAsync(Guid gameId, string theme)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            var answers = game.Players.GetAnswers(game.CurrentRoundNumber);

            var playersValidations = new List<(Guid PlayerId, ICollection<Validation> Validations, int TotalValidations, int ValidationNumber)>();
            foreach (var player in game.Players)
            {
                if (player.InRound)
                {
                    var totalValidations = answers.GetTotalThemesForPlayerValidate(player.Id, game.CurrentRoundNumber);
                    var playerValidations = answers.BuildValidationsForPlayer(player.Id, theme, game.CurrentRound.SortedLetter).ToList();
                    playersValidations.Add((player.Id, playerValidations, totalValidations, player.GetTotalValidationsInRound(game.CurrentRoundNumber) + 1));
                }
            }

            return playersValidations;
        }

        public async Task<bool> CheckAllPlayersSendValidationsAsync(Guid gameId, int roundNumber, string theme)
        {
            var players = await _playerStorage.GetPlayersInRoundAsync(gameId);

            foreach (var player in players)
            {
                if (player.InRound)
                {
                    if (players.IsValidationsRequiredForPlayer(player.Id, roundNumber, theme) && !player.HasValidatedTheme(roundNumber, theme))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task FinishValidationsForThemeAsync(Guid gameId, string theme)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            game.CurrentRound.ValidatedThemes.Add(theme);
            await _gameStorage.EditAsync(game);
        }

        public async Task<string> StartValidationForNextThemeAsync(Guid gameId)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            foreach (var theme in game.Options.Themes)
            {
                if (game.Players.HasAnyAnswerForTheme(game.CurrentRoundNumber, theme) && !game.CurrentRound.ValidatedThemes.Contains(theme))
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

        public async Task FinishCurrentRoundAsync(Guid gameId, Action<Game> finishedRoundAction)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);
            game.FinishRound();

            await _roundScorer.ProcessRoundPontuationAsync(game.CurrentRound);

            // Precisa atualizar os jogadores da partida com suas pontuações já processadas
            game.Players = await _playerStorage.GetAllAsync(gameId);
            await PutAllPlayersInWaiting();

            if (game.IsFinalRound())
            {
                game.Finish();
            }

            await _gameStorage.EditAsync(game);
            finishedRoundAction?.Invoke(game);

            async Task PutAllPlayersInWaiting()
            {
                foreach (var player in game.Players.PutAllPlayersInWaiting())
                {
                    await _playerStorage.EditAsync(player);
                }
            }
        }

        public async Task<bool> ChangeAdminAsync(Guid gameId, Guid newAdminPlayerId)
        {
            var player = await _playerStorage.GetAsync(gameId, newAdminPlayerId);
            if (player != null)
            {
                player.GiveAdmin();
                await _playerStorage.EditAsync(player);
                return true;
            }

            return false;
        }

        public async Task LeaveAsync(Guid gameId, Guid playerId, Action<bool, Player> onLeaveAction)
        {
            var player =  await _playerStorage.GetAsync(gameId, playerId);
            await _playerStorage.DeleteAsync(gameId, playerId);
            
            var game = await _gameStorage.GetByIdAsync(gameId);

            if (!game.Players.Any())
            {
                await _gameStorage.DeleteAsync(gameId);
                onLeaveAction?.Invoke(true, null);
                return;
            }
            else if (player.IsAdmin)
            {
                var newAdmin = game.Players.GetOldestPlayerInGame();
                await ChangeAdminAsync(gameId, newAdmin.Id);
                onLeaveAction?.Invoke(false, newAdmin);
                return;
            }
            else
            {
                onLeaveAction?.Invoke(false, null);
            }
        }
    }
}