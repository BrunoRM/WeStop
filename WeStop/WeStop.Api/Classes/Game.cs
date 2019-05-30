using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public class Game
    {
        public Game(string name, string password, GameOptions options)
        {
            Id = Guid.NewGuid();
            Name = name;
            Password = password;
            Options = options;
            Players = new List<Player>();
            Rounds = new List<Round>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public GameOptions Options { get; set; }
        public ICollection<Round> Rounds { get; set; }
        public ICollection<Player> Players { get; set; }
        public Round CurrentRound { get; private set; }

        public void AddPlayer(Player player)
        {
            if (!Players.Any(x => x.User.Id == player.User.Id))
                Players.Add(player);
        }

        public PlayerRound GetPlayerCurrentRound(Guid playerId)
        {
            return CurrentRound.Players.First(x => x.Player.User.Id == playerId);
        }

        public void StartNextRound()
        {
            string sortedLetter = Options.AvailableLetters[new Random().Next(0, Options.AvailableLetters.Length - 1)];

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

            Rounds.Add(CurrentRound);
        }

        public bool AllPlayersSendValidationsOfTheme(string theme)
        {
            foreach (var player in Players)
            {
                // Verificar se algum outro jogador respondeu esse tema
                var otherPlayersAnsweredTheme = CurrentRound.Players.Any(x => x.Player.User.Id != player.User.Id && x.Answers.Where(a => a.Theme == theme).Any());

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
                    .Where(playerRound => playerRound.Player.User.Id != player.User.Id)
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
            var playersValidations = CurrentRound.Players
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
            CurrentRound.Number == Options.Rounds;

        public ICollection<PlayerScore> GetScoreboard()
        {
            return CurrentRound.Players.Select(x => new PlayerScore
            {
                PlayerId = x.Player.User.Id,
                UserName = x.Player.User.UserName,
                RoundPontuation = x.EarnedPoints,
                GamePontuation = x.Player.EarnedPoints
            }).OrderByDescending(x => x.RoundPontuation).ToList();
        }

        public FinalScoreboard GetFinalPontuation() // Falta tratar empate
        {
            var playersPontuations = new List<PlayerFinalPontuation>();

            foreach (var round in Rounds)
            {
                foreach (var playerRound in round.Players)
                {
                    var playerPontuation = playersPontuations.FirstOrDefault(p => p.UserName == playerRound.Player.User.UserName);

                    if (playerPontuation is null)
                    {
                        playersPontuations.Add(new PlayerFinalPontuation
                        {
                            PlayerId = playerRound.Player.User.Id,
                            UserName = playerRound.Player.User.UserName,
                            Pontuation = playerRound.EarnedPoints
                        });
                    }
                    else
                        playerPontuation.Pontuation += playerRound.EarnedPoints;
                }
            }

            return new FinalScoreboard
            {
                Winner = playersPontuations.OrderByDescending(x => x.Pontuation).First().UserName,
                PlayersPontuations = playersPontuations.OrderByDescending(x => x.Pontuation).ToList()
            };
        }

        public void StartNew()
        {

        }
    }
}