using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Exceptions;

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

            string sortedLetter = Options.AvailableLetters
                .Where(x => x.Value == false).ToArray()[new Random().Next(0, Options.AvailableLetters.Count(al => al.Value == false) - 1)]
                .Key;

            Options.AvailableLetters[sortedLetter] = true;

            CurrentRound = new Round
            {
                Number = Rounds.Any() ? Rounds.Last().Number + 1 : 1,
                Finished = false,
                SortedLetter = sortedLetter,
                Players = this.Players.Select(x => new PlayerRound
                {
                    Player = x
                }).ToList()
            };

            _rounds.Add(CurrentRound);
        }

        public bool AllPlayersSendValidationsOfTheme(string theme)
        {
            foreach (var player in Players)
            {
                // Verificar se algum outro jogador respondeu esse tema
                var otherPlayersAnsweredTheme = CurrentRound.Players.Any(x => x.Player.User.Id != player.User.Id && player.Status == PlayerStatus.Online && x.Answers.Where(a => a.Theme == theme).Any());

                // Se sim, o jogador da iteração atual deverá ter validado a resposta
                if (otherPlayersAnsweredTheme)
                {
                    if (!GetPlayerCurrentRound(player.User.Id).ThemesAnswersValidations.Any(themeValidation => themeValidation.Theme == theme))
                        return false;
                }
            }

            return true;
        }

        public bool AllPlayersSendValidationsOfAllThemes()
        {
            // Como um jogador ou mais podem acabar não informando resposta para algum dos N temas, 
            // para cada jogador da rodada atual serão filtrados os temas para quais todos os outros
            // jogadores informaram resposta, sendo assim, o jogador corrente na iteração terá de ter
            // validado essas respostas
            foreach (var player in Players)
            {
                var themesWithPlayersAnswers = CurrentRound.Players
                    .Where(playerRound => playerRound.Player.User.Id != player.User.Id && player.Status == PlayerStatus.Online)
                    .Select(playerRound => playerRound.Answers)
                    .SelectMany(answers => answers.Select(x => x.Theme)).Distinct();

                foreach (var theme in themesWithPlayersAnswers)
                {
                    if (!AllPlayersSendValidationsOfTheme(theme))
                        return false;
                }
            }

            return true;
        }

        public void ProccessPontuationForTheme(string theme)
        {
            // Buscar as validações dos jogadores para as respostas desse tema na rodada atual
            var playersValidations = CurrentRound.GetPlayersOnline()
                .Select(x => x.ThemesAnswersValidations)
                .Select(x => x.Where(y => y.Theme == theme))
                .SelectMany(x => x.SelectMany(y => y.AnswersValidations));

            // Verificar se a resposta é válida para a maioria dos jogadores
            var validations = new Dictionary<string, ICollection<bool>>();

            foreach (var validation in playersValidations)
            {
                if (validations.ContainsKey(validation.Answer))
                    validations[validation.Answer].Add(validation.Valid);
                else
                    validations.Add(validation.Answer, new List<bool> { validation.Valid });
            }

            foreach (var answerValidations in validations)
            {
                if (answerValidations.Value.Count(x => x == true) >= answerValidations.Value.Count(x => x == false))
                {
                    // Se for válida para a maioria, Verificar quantos jogadores informaram esse tema
                    var players = CurrentRound.Players
                        .Where(x => x.Answers.Where(y => y.Theme == theme && y.Answer == answerValidations.Key).Count() > 0);

                    // Se for mais de um, dar 5 pontos para cada jogador
                    if (players.Count() > 1)
                    {
                        foreach (var player in players)
                            player.GeneratePointsForTheme(theme, 5);
                    }
                    else
                    {
                        foreach (var player in players)
                            player.GeneratePointsForTheme(theme, 10);
                    }
                }
                else
                {
                    var players = CurrentRound.Players
                        .Where(x => x.Answers.Where(y => y.Theme == theme && y.Answer == answerValidations.Key).Count() > 0);

                    foreach (var player in players)
                        player.GeneratePointsForTheme(theme, 0);
                }
            }

            // Busca os jogadores que não informaram resposta para esse tema e gera pontuação 0 para eles
            var playersWithBlankThemeAnswer = CurrentRound.Players.Where(x => !x.Answers.Where(y => y.Theme == theme).Any());

            foreach (var player in playersWithBlankThemeAnswer)
                player.GeneratePointsForTheme(theme, 0);
        }

        public bool IsFinalRound() =>
            CurrentRound?.Number == Options.Rounds;

        public ICollection<PlayerScore> GetScoreboard()
        {
            return CurrentRound?.GetPlayersOnline().Select(x => new PlayerScore
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

        public void StartNew()
        {

        }
    }
}