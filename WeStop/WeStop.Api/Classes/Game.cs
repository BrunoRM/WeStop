using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Exceptions;
using WeStop.Api.Extensions;

namespace WeStop.Api.Classes
{
    public class Game
    {
        private ICollection<Player> _players;
        private ICollection<Round> _rounds;

        public Game(string name, string password, GameOptions options)
        {
            Id = Guid.NewGuid();
            Name = name;
            Password = password;
            Options = options;
            _players = new List<Player>();
            _rounds = new List<Round>();
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Password { get; private set; }
        public GameOptions Options { get; private set; }
        public IReadOnlyCollection<Round> Rounds => _rounds.ToList();
        public IReadOnlyCollection<Player> Players => _players.ToList();
        public Round CurrentRound { get; private set; }

        public int GetNextRoundNumber() =>
            CurrentRound?.Number + 1 ?? 1;

        public void AddPlayer(Player player)
        {
            if (!Players.Any(x => x.User.Id == player.User.Id))
                _players.Add(player);
        }

        public Player GetPlayer(Guid id) =>
            _players.FirstOrDefault(p => p.Id == id);

        public PlayerRound GetPlayerCurrentRound(Guid playerId) =>
            CurrentRound.Players.First(x => x.Player.User.Id == playerId);

        public void StartNextRound()
        {
            if (IsFinalRound())
                throw new WeStopException("O jogo já chegou ao fim. Não é possível iniciar a rodada");

            CurrentRound = CreateNewRound();
        }

        private Round CreateNewRound()
        {
            int nextRoundNumber = GetNextRoundNumber();
            string drawnLetter = SortOutLetter();
            ICollection<PlayerRound> players = GetPlayersOnlineAndReadyForNewRound();
            Round newRound = new Round(nextRoundNumber, drawnLetter, players);

            _rounds.Add(newRound);
            return newRound;
        }

        private string SortOutLetter()
        {
            string sortedLetter = Options.AvailableLetters
                            .Where(x => x.Value == false).ToArray()[new Random().Next(0, Options.AvailableLetters.Count(al => al.Value == false) - 1)]
                            .Key;

            Options.AvailableLetters[sortedLetter] = true;
            return sortedLetter;
        }

        private ICollection<PlayerRound> GetPlayersOnlineAndReadyForNewRound()
        {
            return Players.Where(p => p.IsReady && p.Status == PlayerStatus.Online)
                .Select(x => new PlayerRound
                {
                    Player = x
                }).ToList();
        }

        public bool AllPlayersSendValidationsOfTheme(string theme)
        {
            foreach (var player in Players)
            {
                var otherPlayersAnsweredTheme = AnyOtherPlayersRepliedForTheme(player.Id, theme);

                if (otherPlayersAnsweredTheme)
                {
                    var playerCurrentRound = GetPlayerCurrentRound(player.Id);
                    if (!playerCurrentRound.HasValidationForTheme(theme))
                        return false;
                }
            }

            return true;
        }

        private bool AnyOtherPlayersRepliedForTheme(Guid playerId, string theme) =>
            CurrentRound.Players.Any(x => x.Player.User.Id != playerId && x.Player.Status == PlayerStatus.Online && x.Answers.Where(a => a.Theme == theme).Any());

        public bool AllPlayersSendValidationsOfAllThemes()
        {
            // Como um jogador ou mais podem acabar não informando resposta para algum dos N temas, 
            // para cada jogador da rodada atual serão filtrados os temas para quais todos os outros
            // jogadores informaram resposta, sendo assim, o jogador corrente na iteração terá de ter
            // validado essas respostas
            foreach (var player in Players)
            {
                var themesWithPlayersAnswers = GetThemesThatHasOtherPlayersAnswers(player.Id);

                foreach (var theme in themesWithPlayersAnswers)
                {
                    if (!AllPlayersSendValidationsOfTheme(theme))
                        return false;
                }
            }

            return true;
        }

        private IEnumerable<string> GetThemesThatHasOtherPlayersAnswers(Guid playerId)
        {
            return CurrentRound.Players
                    .Where(playerRound => playerRound.Player.User.Id != playerId && playerRound.Player.Status == PlayerStatus.Online)
                    .Select(playerRound => playerRound.Answers)
                    .SelectMany(answers => answers.Select(x => x.Theme)).Distinct();
        }

        // TODO: Revisar nomes dos métodos e quebrar mais eles
        public void ProccessPontuationForTheme(string theme)
        {
            var playersValidations = GetOnlinePlayersValidationsForTheme(theme);
            var groupedValidations = GroupTotalValidationsByAnswer(playersValidations);

            foreach (var answerValidations in groupedValidations)
            {
                if (TotalValidIsGreatherThanOrEqualToTotalInvalidForAnswer(answerValidations))
                {
                    var players = GetPlayersThatRepliedAnswerForTheme(answer: answerValidations.Key, theme);

                    if (players.MoreThanOnePlayerRepliedAnswerForTheme())
                        GenerateFiveThemePointsForEachPlayer(theme, players);
                    else
                        GenerateTenThemePointsForEachPlayer(theme, players);
                }
                else
                {
                    var players = GetPlayersThatRepliedAnswerForTheme(answerValidations.Key, theme);
                    GenerateZeroThemePointsForEachPlayer(theme, players);
                }
            }

            var playersWithBlankThemeAnswer = GetPlayersThatNotReportAnswerForTheme(theme);
            GenerateZeroThemePointsForEachPlayer(theme, playersWithBlankThemeAnswer);
        }

        private ICollection<PlayerRound> GetPlayersThatNotReportAnswerForTheme(string theme) =>
            CurrentRound.Players.Where(x => !x.Answers.Where(y => y.Theme == theme).Any()).ToList();

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
            return CurrentRound.Players
                .Where(player => player.Answers.Where(y => y.Theme == theme && y.Answer == answer).Count() > 0).ToList();
        }

        private bool TotalValidIsGreatherThanOrEqualToTotalInvalidForAnswer(KeyValuePair<string, ICollection<bool>> answerValidations) =>
            answerValidations.Value.Count(x => x == true) >= answerValidations.Value.Count(x => x == false);

        private Dictionary<string, ICollection<bool>> GroupTotalValidationsByAnswer(ICollection<AnswerValidation> answersValidations)
        {
            var groupedValidations = new Dictionary<string, ICollection<bool>>();
            foreach (var validation in answersValidations)
            {
                if (groupedValidations.ContainsKey(validation.Answer))
                    groupedValidations[validation.Answer].Add(validation.Valid);
                else
                    groupedValidations.Add(validation.Answer, new List<bool> { validation.Valid });
            }

            return groupedValidations;
        }

        private ICollection<AnswerValidation> GetOnlinePlayersValidationsForTheme(string theme)
        {
            return CurrentRound.GetOnlinePlayers()
                .Select(playerRound => playerRound.ThemesAnswersValidations)
                .Select(themeValidations => themeValidations.Where(y => y.Theme == theme))
                .SelectMany(themeValidations => themeValidations.SelectMany(themeValidation => themeValidation.AnswersValidations))
                .ToList();
        }

        public bool IsFinalRound() =>
            CurrentRound?.Number == Options.Rounds;

        public ICollection<PlayerScore> GetScoreboard()
        {
            return CurrentRound?.GetOnlinePlayers().Select(x => new PlayerScore
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

            // Busca a melhor pontuação entre todos os jogadores
            int bestPontuation = scoreBoard.Max(p => p.GamePontuation);

            // Busca o nome de todos os jogadores que tiveram a pontuação igual a melhor
            return scoreBoard
                .Where(sb => sb.GamePontuation == bestPontuation)
                .OrderBy(p => p.UserName)
                .Select(sb => sb.UserName);
        }

        public bool AllOnlinePlayersSendAnswers() =>
            !CurrentRound.Players.Any(p => p.Player.Status == PlayerStatus.Online && !p.AnswersSended);
    }
}