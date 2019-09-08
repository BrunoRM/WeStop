using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Exceptions;
using WeStop.Api.Extensions;

namespace WeStop.Api.Classes
{
    public sealed class Game
    {
        private ICollection<Player> _players;
        private ICollection<Round> _rounds;
        private Round _currentRound;
        private GameState _currentState;

        public Game(string name, string password, GameOptions options)
        {
            Id = Guid.NewGuid();
            Name = name;
            Password = password;
            Options = options;
            _players = new List<Player>();
            _rounds = new List<Round>();
            _currentState = GameState.Waiting;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Password { get; private set; }
        public GameOptions Options { get; private set; }
        public Round CurrentRound { get; private set; }
        public IReadOnlyCollection<Round> Rounds => _rounds.ToList();
        public IReadOnlyCollection<Player> Players => _players.ToList();

        #region Public Methods

        public int GetPlayersCount() =>
            Players.Count();

        public bool IsPlayerAdmin(Guid playerId) =>
            Players.FirstOrDefault(x => x.Id == playerId).IsAdmin;

        public bool HasSuficientPlayersToStartNewRound() =>
            GetPlayersCount() >= 2;

        public int GetNextRoundNumber() =>
            _currentRound?.Number + 1 ?? 1;

        public int GetCurrentRoundNumber() =>
            _currentRound?.Number ?? 1;

        public void AddPlayer(Player player)
        {
            if (!Players.Any(x => x.User.Id == player.User.Id))
                _players.Add(player);
        }

        public Player GetPlayer(Guid id) =>
            _players.FirstOrDefault(p => p.Id == id);        

        public void StartNextRound()
        {
            if (IsFinalRound())
                throw new WeStopException("O jogo já chegou ao fim. Não é possível iniciar a rodada");

            _currentRound = CreateNewRound();
            CurrentRound = CreateNewRound();
            _currentState = GameState.InProgress;
        }        

        public string[] GetThemes() =>
            Options.Themes;

        public bool IsFinalRound() =>
            _currentRound?.Number == Options.Rounds;

        public string GetCurrentRoundSortedLetter() =>
            _currentRound.SortedLetter;

        public IEnumerable<ThemeValidation> GetDefaultValidationsForPlayer(Guid playerId)
        {
            var answersThatPlayerShouldValidate = GetCurrentRoundPlayersAnswersExceptFromPlayer(playerId);

            var playerCurrentRound = GetPlayerCurrentRound(playerId);
            foreach (var themeAnswers in answersThatPlayerShouldValidate)
            {
                if (themeAnswers.Answers.Any(a => !string.IsNullOrEmpty(a)))
                {
                    ThemeValidation themeValidation = new ThemeValidation(themeAnswers.Theme, themeAnswers.Answers
                        .Where(a => !string.IsNullOrEmpty(a))
                        .Select(a => new AnswerValidation(a, true))
                        .ToList());

                    yield return themeValidation;
                }
            }
        }

        public ThemeValidation GetDefaultValidationsOfThemeForPlayer(string theme, Guid playerId)
        {
            var answersThatPlayerShouldValidate = GetCurrentRoundPlayersAnswersForThemeExceptFromPlayer(theme, playerId);

            var playerCurrentRound = GetPlayerCurrentRound(playerId);
            ThemeValidation themeValidation = new ThemeValidation(theme, answersThatPlayerShouldValidate.Answers.Where(a => !string.IsNullOrEmpty(a))
                .Select(a => new AnswerValidation(a, true)).ToList());

            return themeValidation;
        }

        public ICollection<ThemeValidation> BuildDefaultValidationForPlayer(Guid playerId)
        {
            var answersThatPlayerShouldValidate = GetCurrentRoundPlayersAnswersExceptFromPlayer(playerId);

            var playerCurrentRound = GetPlayerCurrentRound(playerId);
            foreach (var themeAnswers in answersThatPlayerShouldValidate)
            {
                if (themeAnswers.Answers.Any(a => !string.IsNullOrEmpty(a)))
                {
                    ThemeValidation themeValidation = new ThemeValidation(themeAnswers.Theme, themeAnswers.Answers
                        .Where(a => !string.IsNullOrEmpty(a))
                        .Select(a => new AnswerValidation(a, true))
                        .ToList());

                    playerCurrentRound.AddThemeAnswersValidations(themeValidation);
                }
            }

            return playerCurrentRound.GetThemeValidations();
        }

        public void SetDefaultValidationsOfThemeForPlayer(string theme, Guid playerId)
        {
            var answersThatPlayerShouldValidate = GetCurrentRoundPlayersAnswersForThemeExceptFromPlayer(theme, playerId);

            var playerCurrentRound = GetPlayerCurrentRound(playerId);
            ThemeValidation themeValidation = new ThemeValidation(theme, answersThatPlayerShouldValidate.Answers.Where(a => !string.IsNullOrEmpty(a))
                .Select(a => new AnswerValidation(a, true)).ToList());

            playerCurrentRound.AddThemeAnswersValidations(themeValidation);
        }

        public void AddPlayerValidationsForTheme(Guid playerId, string theme, ICollection<AnswerValidation> validations)
        {
            var playerCurrentRound = GetPlayerCurrentRound(playerId);
            playerCurrentRound.AddValidatiosForTheme(theme, validations);
        }

        public void SetDefaultThemeValidationsForPlayersThatHasNotAnyValidations(string theme)
        {
            var playersThatNotValidatedTheme = _currentRound.Players.Where(p => !p.ThemesAnswersValidations.Any(a => a.Theme == theme)).Select(p => p.Player);

            if (playersThatNotValidatedTheme.Any())
            {
                foreach (var player in playersThatNotValidatedTheme)
                {
                    SetDefaultValidationsOfThemeForPlayer(theme, player.Id);
                }
            }
        }

        public bool IsAllThemesValidated()
        {
            foreach (string theme in Options.Themes)
            {
                if (AnyPlayerHasNotValidatedTheme(theme))
                {
                    return false;
                }
            }

            return true;
        }

        public string GetThemeNotValidatedYet()
        {
            foreach (string theme in Options.Themes)
            {
                if (AnyPlayerHasNotValidatedTheme(theme))
                {
                    return theme;
                }
            }

            throw new WeStopException("Todos os temas já foram validados");
        }

        public GameState GetCurrentState()
        {
            return _currentState;
        }

        public void BeginCurrentRoundThemesValidations()
        {
            _currentState = GameState.ThemesValidations;
        }

        public void FinishRound()
        {
            // TODO: verificar se existe algum jogador que não possui validação para algum tema, e gerar as validações padrão para o mesmo
            _currentState = GameState.Waiting;
        }

        public void Finish()
        {
            if (!IsFinalRound())
                throw new WeStopException("O jogo só poderá ser finalizado se a rodada atual for a última");

            _currentState = GameState.Finished;
        }        

        public bool AllPlayersSendValidationsOfTheme(string theme)
        {
            foreach (var player in Players)
            {
                bool otherPlayersAnsweredTheme = AnyOtherPlayersRepliedForTheme(player.Id, theme);

                if (otherPlayersAnsweredTheme)
                {
                    var playerCurrentRound = GetPlayerCurrentRound(player.Id);
                    if (!playerCurrentRound.HasValidationForTheme(theme))
                        return false;
                }
            }

            return true;
        }

        public bool HasPlayerValidatedTheme(Guid playerId, string theme)
        {
            var playerCurrentRound = GetPlayerCurrentRound(playerId);
            return playerCurrentRound.HasValidationForTheme(theme);
        }

        public ICollection<Player> GetPlayers() =>
            Players.ToList();

        #endregion

        #region Private Methods

        private bool AnyOtherPlayersRepliedForTheme(Guid playerId, string theme) =>
            _currentRound.Players.Any(x => x.Player.User.Id != playerId && x.Answers.Where(a => a.Theme == theme).Any());

        private bool AnyPlayerHasNotValidatedTheme(string theme) =>
            _currentRound.Players.Any(p => !p.HasValidationForTheme(theme));

        private Round CreateNewRound()
        {
            int nextRoundNumber = GetNextRoundNumber();
            string drawnLetter = SortOutLetter();
            Round newRound = new Round(nextRoundNumber, drawnLetter);

            _rounds.Add(newRound);
            return newRound;
        }

        private string SortOutLetter()
        {
            string[] notSortedLetters = GetNotSortedLetters();

            Random random = new Random();
            int randomSortedIndex = random.Next(0, notSortedLetters.Count() - 1);

            string sortedLetter = notSortedLetters[randomSortedIndex];

            Options.AvailableLetters[sortedLetter] = true;
            return sortedLetter;
        }

        private string[] GetNotSortedLetters() =>
            Options.AvailableLetters.Where(al => al.Value == false).Select(al => al.Key).ToArray();

        #endregion
    }
}