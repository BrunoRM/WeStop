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
        public IReadOnlyCollection<Round> Rounds => _rounds.ToList();
        public IReadOnlyCollection<Player> Players => _players.ToList();

        public int GetPlayersCount() =>
            Players.Count();

        public bool IsPlayerAdmin(Guid playerId) =>
            Players.FirstOrDefault(x => x.Id == playerId).IsAdmin;

        public bool HasSuficientPlayersToStartNewRound() =>
            GetPlayersCount() >= 2;

        public int GetNextRoundNumber() =>
            _currentRound?.Number + 1 ?? 1;

        public void AddPlayer(Player player)
        {
            if (!Players.Any(x => x.User.Id == player.User.Id))
                _players.Add(player);
        }

        public Player GetPlayer(Guid id) =>
            _players.FirstOrDefault(p => p.Id == id);

        private PlayerRound GetPlayerCurrentRound(Guid playerId) =>
            _currentRound.Players.First(x => x.Player.Id == playerId);

        public void StartNextRound()
        {
            if (IsFinalRound())
                throw new WeStopException("O jogo já chegou ao fim. Não é possível iniciar a rodada");

            _currentRound = CreateNewRound();
            _currentState = GameState.InProgress;
        }

        private Round CreateNewRound()
        {
            int nextRoundNumber = GetNextRoundNumber();
            string drawnLetter = SortOutLetter();
            ICollection<PlayerRound> players = GetPlayersReadyForNewRound();
            Round newRound = new Round(nextRoundNumber, drawnLetter, players);

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

        private ICollection<PlayerRound> GetPlayersReadyForNewRound()
        {
            return Players.Where(p => p.IsReady)
                .Select(x => new PlayerRound
                {
                    Player = x
                }).ToList();
        }

        public string[] GetThemes() =>
            Options.Themes;

        public void GeneratePontuationForTheme(string theme)
        {
            string[] answersForTheme = GetPlayersAnswersForTheme(theme);

            ICollection<PlayerRound> _currentRoundPlayers = _currentRound.GetPlayers();
            foreach (var answer in answersForTheme)
            {
                int validVotesCountForThemeAnswer = _currentRoundPlayers.GetValidVotesCountForThemeAnswer(theme, answer);
                int invalidVotesCountForThemeAnswer = _currentRoundPlayers.GetInvalidVotesCountForThemeAnswer(theme, answer);

                if (validVotesCountForThemeAnswer >= invalidVotesCountForThemeAnswer)
                {
                    var players = GetPlayersThatRepliedAnswerForTheme(answer, theme);

                    if (!players.Any())
                    {
                        continue;
                    }

                    if (players.Count() > 1)
                    {
                        GenerateFiveThemePointsForEachPlayer(theme, players);
                    }
                    else
                    {
                        GenerateTenThemePointsForEachPlayer(theme, players);
                    }
                }
                else
                {
                    var players = GetPlayersThatRepliedAnswerForTheme(answer, theme);
                    GenerateZeroThemePointsForEachPlayer(theme, players);
                }
            }

            var playersWithBlankThemeAnswer = GetPlayersThatNotRepliedAnswerForTheme(theme);
            GenerateZeroThemePointsForEachPlayer(theme, playersWithBlankThemeAnswer);
        }

        private string[] GetPlayersAnswersForTheme(string theme)
        {
            return _currentRound.GetPlayers()
                .SelectMany(p => p.Answers.Where(a => a.Theme == theme && !string.IsNullOrEmpty(a.Answer)).Select(a => a.Answer)).Distinct().ToArray();
        }

        private ICollection<PlayerRound> GetPlayersThatNotRepliedAnswerForTheme(string theme)
        {
            return _currentRound.GetPlayers()
                .Where(p => !p.Answers.Where(a => a.Theme == theme).Any() || p.Answers.Where(a => a.Theme == theme && string.IsNullOrEmpty(a.Answer)).Any()).ToList();
        }

        private void GenerateFiveThemePointsForEachPlayer(string theme, ICollection<PlayerRound> players)
        {
            foreach (var player in players)
                player.GeneratePointsForTheme(theme, 5);
        }

        private void GenerateTenThemePointsForEachPlayer(string theme, ICollection<PlayerRound> players)
        {
            foreach (var player in players)
                player.GeneratePointsForTheme(theme, 10);
        }

        private void GenerateZeroThemePointsForEachPlayer(string theme, ICollection<PlayerRound> players)
        {
            foreach (var player in players)
                player.GeneratePointsForTheme(theme, 0);
        }

        private ICollection<PlayerRound> GetPlayersThatRepliedAnswerForTheme(string answer, string theme)
        {
            return _currentRound.Players
                .Where(player => player.Answers.Where(y => y.Theme == theme && y.Answer == answer).Count() > 0).ToList();
        }

        public bool IsFinalRound() =>
            _currentRound?.Number == Options.Rounds;

        public ICollection<PlayerScore> GetScoreboard()
        {
            return _currentRound?.GetPlayers().Select(x => new PlayerScore
            {
                PlayerId = x.Player.User.Id,
                UserName = x.Player.User.UserName,
                RoundPontuation = x.EarnedPoints,
                GamePontuation = x.Player.EarnedPoints
            }).OrderByDescending(x => x.GamePontuation).ToList();
        }

        public IEnumerable<string> GetWinners()
        {
            if (!IsFinalRound())
                throw new WeStopException("Só é possível eleger um vencedor se a partida já foi finalizada");

            var scoreBoard = GetScoreboard();

            int bestPontuation = scoreBoard.Max(p => p.GamePontuation);

            return scoreBoard
                .Where(sb => sb.GamePontuation == bestPontuation)
                .OrderBy(p => p.UserName)
                .Select(sb => sb.UserName);
        }

        public void AddPlayerAnswerForTheme(Guid playerId, string theme, string answer)
        {
            if (Options.Themes.Contains(theme))
            {
                PlayerRound playerCurrentRound = GetPlayerCurrentRound(playerId);
                playerCurrentRound.AddAnswerForTheme(theme, answer);
            }
        }

        public void AddPlayerAnswersValidations(Guid playerId, ThemeValidation validations)
        {
            var playerCurrentRound = GetPlayerCurrentRound(playerId);
            playerCurrentRound.AddThemeAnswersValidations(validations);
        }

        public void AddPlayerAnswersValidations(Guid playerId, ICollection<ThemeValidation> validations)
        {
            foreach (var validation in validations)
                AddPlayerAnswersValidations(playerId, validation);
        }

        public int GetPlayerCurrentRoundPontuationForTheme(Guid playerId, string theme)
        {
            var playerCurrentRound = GetPlayerCurrentRound(playerId);
            int playerPontuationForTheme = playerCurrentRound.ThemesPontuations[theme];

            return playerPontuationForTheme;
        }

        public int GetPlayerCurrentRoundEarnedPoints(Guid playerId)
        {
            var playerCurrentRound = GetPlayerCurrentRound(playerId);

            return playerCurrentRound.EarnedPoints;
        }

        public string GetCurrentRoundSortedLetter() =>
            _currentRound.SortedLetter;

        public ICollection<ThemeAnswers> GetCurrentRoundPlayersAnswersExceptFromPlayer(Guid playerId) =>
            _currentRound.GetPlayersAnswersExceptFromPlayer(playerId);

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

        public IEnumerable<ThemeValidation> GetDefaultValidationsOfThemeForPlayer(string theme, Guid playerId)
        {
            var answersThatPlayerShouldValidate = GetCurrentRoundPlayersAnswersForThemeExceptFromPlayer(theme, playerId);

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

        private ICollection<ThemeAnswers> GetCurrentRoundPlayersAnswersForThemeExceptFromPlayer(string theme, Guid playerId) =>
            _currentRound.GetCurrentRoundPlayersAnswersForThemeExceptFromPlayer(theme, playerId);

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

        public ICollection<ThemeValidation> SetDefaultValidationsOfThemeForPlayer(string theme, Guid playerId)
        {
            var themeAnswersThatPlayerShouldValidate = GetCurrentRoundPlayersAnswersForThemeExceptFromPlayer(theme, playerId);

            var playerCurrentRound = GetPlayerCurrentRound(playerId);
            foreach (var themeAnswers in themeAnswersThatPlayerShouldValidate)
            {
                if (themeAnswers.Answers.Any(a => !string.IsNullOrEmpty(a)))
                {
                    ThemeValidation themeValidation = new ThemeValidation(theme, themeAnswers.Answers
                        .Where(a => !string.IsNullOrEmpty(a))
                        .Select(a => new AnswerValidation(a, true))
                        .ToList());

                    playerCurrentRound.AddThemeAnswersValidations(themeValidation);
                }
            }

            return playerCurrentRound.GetThemeValidations();
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

        private bool AnyPlayerHasNotValidatedTheme(string theme) =>
            _currentRound.Players.Any(p => !p.HasValidationForTheme(theme));

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
            _currentState = GameState.Waiting;
        }

        public void Finish()
        {
            if (!IsFinalRound())
                throw new WeStopException("O jogo só poderá ser finalizado se a rodada atual for a última");

            _currentState = GameState.Finished;
        }

        private bool AnyOtherPlayersRepliedForTheme(Guid playerId, string theme) =>
            _currentRound.Players.Any(x => x.Player.User.Id != playerId && x.Answers.Where(a => a.Theme == theme).Any());

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
    }
}