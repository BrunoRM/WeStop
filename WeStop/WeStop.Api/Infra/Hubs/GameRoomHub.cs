using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Domain.Errors;

namespace WeStop.Api.Infra.Hubs
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
    }

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
            if (!Players.Any(x => x.Id == player.Id))
                Players.Add(player);
        }

        public PlayerRound GetPlayerCurrentRound(Guid playerId)
        {
            return CurrentRound.Players.First(x => x.Player.Id == playerId);
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
                if (!GetPlayerCurrentRound(player.Id).ThemesAnswersValidations.Any(themeValidation => themeValidation.Theme == theme))
                    return false;
            }

            return true;
        }

        public bool AllPlayersSendValidationsOfAllThemes()
        {
            foreach (var player in Players)
            {
                foreach (var theme in Options.Themes)
                {
                    if (!GetPlayerCurrentRound(player.Id).ThemesAnswersValidations.Any(themeValidation => themeValidation.Theme == theme))
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
                .Select(x => x.First(y => y.Theme == theme))
                .Select(x => x.AnswersValidations);

            // Verificar se a resposta é válida para a maioria dos jogadores
            var validations = new Dictionary<string, ICollection<bool>>();
            foreach (var playerValidation in playersValidations)
            {
                foreach (var validation in playerValidation)
                {
                    if (validations.ContainsKey(validation.Answer))
                        validations[validation.Answer].Add(validation.Valid);
                    else
                        validations.Add(validation.Answer, new List<bool> { validation.Valid });
                }
            }

            foreach (var answerValidations in validations)
            {
                if (answerValidations.Value.Count(x => x == true) >= answerValidations.Value.Count(x => x == false))
                {
                    // Se for válida para a maioria, Verificar quantos jogadores informaram esse tema
                    var players = CurrentRound.Players
                        .Where(x => x.Answers.Where(y => y.Theme == theme && y.Answer == answerValidations.Key).Count() > 0);

                    // Se for mais de um, dar 5 pontos para cada jogador
                    if (players?.Count() > 1)
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

        }

        public void ChangeStatusOfAllPlayersToWait()
        {
            foreach (var player in Players)
                player.IsReady = false;
        }
    }

    public class GameOptions
    {
        public GameOptions(string[] themes, string[] availableLetters, int rounds, int numberOfPlayers)
        {
            Themes = themes;
            AvailableLetters = availableLetters;
            Rounds = rounds;
            NumberOfPlayers = numberOfPlayers;
        }

        public string[] Themes { get; set; }
        public string[] AvailableLetters { get; set; }
        public int Rounds { get; set; }
        public int NumberOfPlayers { get; set; }
    }

    public class Player
    {
        public Player()
        {
            IsReady = false;
            IsAdmin = false;
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsReady { get; set; }
        public int EarnedPoints { get; set; }
    }

    public class ThemeAnswer
    {
        public ThemeAnswer(string theme, string answer)
        {
            Theme = theme;
            Answer = answer;
        }

        public string Theme { get; set; }
        public string Answer { get; set; }
    }

    public class PlayerRound
    {
        public PlayerRound()
        {
            ThemesAnswersValidations = new List<ThemeValidation>();
            ThemesPontuations = new Dictionary<string, int>();
            Answers = new List<ThemeAnswer>();
        }

        public Player Player { get; set; }
        public ICollection<ThemeAnswer> Answers { get; set; }
        public IDictionary<string, int> ThemesPontuations { get; set; }
        public ICollection<ThemeValidation> ThemesAnswersValidations { get; set; }
        public int EarnedPoints => ThemesPontuations.Values.Sum();

        public void AddThemeAnswersValidations(ThemeValidation validation)
        {
            if (!ThemesAnswersValidations.Any(x => x.Theme == validation.Theme))
                ThemesAnswersValidations.Add(validation);
        }

        private void AddAnswer(ThemeAnswer themeAnswer)
        {
            if (Answers.Any(x => x.Theme == themeAnswer.Theme))
                return;

            themeAnswer.Answer = themeAnswer.Answer.Trim();

            Answers.Add(themeAnswer);
        }

        public void AddAnswers(ICollection<ThemeAnswer> answers)
        {
            foreach (var answer in answers)
                AddAnswer(answer);
        }

        public void GeneratePointsForTheme(string theme, int points)
        {
            ThemesPontuations.Add(theme, points);
            Player.EarnedPoints += points;
        }
    }

    public class ThemeValidation
    {
        public ThemeValidation(string theme, ICollection<AnswerValidation> answersValidations)
        {
            Theme = theme;
            AnswersValidations = answersValidations;
        }

        public string Theme { get; set; }
        public ICollection<AnswerValidation> AnswersValidations { get; set; }
    }

    public class AnswerValidation
    {
        public AnswerValidation(string answer, bool valid)
        {
            Answer = answer;
            Valid = valid;
        }

        public string Answer { get; set; }
        public bool Valid { get; set; }
    }

    public class Round
    {
        public Round()
        {
            Finished = false;
            Players = new List<PlayerRound>();
        }

        public int Number { get; set; }
        public string SortedLetter { get; set; }
        public bool Finished { get; set; }
        public ICollection<PlayerRound> Players { get; set; }
    }

    public class PlayerThemeValidation
    {
        public PlayerThemeValidation()
        {
            Validations = new Dictionary<string, bool>();
        }

        public string Theme { get; set; }
        public IDictionary<string, bool> Validations { get; set; }
    }

    public class GameRoomHub : Hub
    {
        private static IDictionary<Guid, Game> _games = new Dictionary<Guid, Game>();
        private static ICollection<User> _users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "Bruno"
            },
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "Gustavo"
            },
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "Giovani"
            },
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "Lucas"
            },
            new User
            {
                Id = Guid.NewGuid(),
                UserName = "Davi"
            }
        };

        public class CreateGameDto
        {
            public string UserName { get; set; }
            public string Name { get; set; }
            public GameOptions GameOptions { get; set; }
        }

        [HubMethodName("games.create")]
        public async Task CreateGame(CreateGameDto dto)
        {
            var user = _users.FirstOrDefault(x => x.UserName == dto.UserName);

            var game = new Game(dto.Name, string.Empty, new GameOptions(dto.GameOptions.Themes, dto.GameOptions.AvailableLetters, dto.GameOptions.Rounds, dto.GameOptions.NumberOfPlayers));

            game.AddPlayer(new Player
            {
                Id = user.Id,
                UserName = user.UserName,
                IsAdmin = true
            });

            _games.Add(game.Id, game);

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            await Clients.Caller.SendAsync("game.created", new
            {
                ok = true,
                is_admin = true,
                game
            });
        }

        [HubMethodName("games.get")]
        public async Task GetGames()
        {
            await Clients.Caller.SendAsync("games.get.response", new
            {
                ok = true,
                games = _games.Values.Where(x => x.Players.Count < x.Options.NumberOfPlayers).Select(x => new
                {
                    x.Id,
                    x.Name,
                    numberOfPlayersAccepted = x.Options.NumberOfPlayers,
                    numberOfPlayers = x.Players.Count
                })
            });
        }

        [HubMethodName("game.join")]
        public async Task Join(JoinToGameRoomDto data)
        {
            var user = _users.FirstOrDefault(x => x.UserName == data.UserName);

            var game = _games[data.GameRoomId];

            var player = game.Players.FirstOrDefault(x => x.Id == user.Id);

            if (player is null)
            {
                player = new Player
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    IsAdmin = false
                };

                game.AddPlayer(player);

            }

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());

            await Clients.Caller.SendAsync("game.player.joined", new
            {
                ok = true,
                player,
                game
            });

            await Clients.GroupExcept(game.Id.ToString(), Context.ConnectionId).SendAsync("game.players.joined", new
            {
                ok = true,
                player
            });
        }

        [HubMethodName("game.startRound")]
        public async Task StartGame(StartGameDto dto)
        {
            var game = _games[dto.GameRoomId];

            if (game is null)
                await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.GameRoomNotFound });

            if (!game.Players.FirstOrDefault(x => x.UserName == dto.UserName).IsAdmin)
                await Clients.Caller.SendAsync("error", new { ok = false, error = GameRoomErrors.NotAdmin });

            if (game.Players.Count() < 2)
                await Clients.Caller.SendAsync("error", new { ok = false, error = "insuficient_players" });

            game.StartNextRound();

            await Clients.Group(game.Id.ToString()).SendAsync("game.roundStarted", new
            {
                ok = true,
                gameRoomConfig = new
                {
                    game.Id,
                    themes = game.Options.Themes,
                    currentRound = game.Rounds.Last()
                }
            });
        }

        [HubMethodName("players.stop")]
        public async Task Stop(StopDto dto)
        {
            var game = _games[dto.GameId];

            var playerCalledStop = game.Players.FirstOrDefault(x => x.UserName == dto.UserName);

            await Clients.Group(dto.GameId.ToString()).SendAsync("players.stopCalled", new
            {
                ok = true,
                userName = playerCalledStop.UserName
            });
        }

        [HubMethodName("player.sendAnswers")]
        public async Task SendAnswers(SendAnswersDto dto)
        {
            var game = _games[dto.GameId];

            var player = game.Players.FirstOrDefault(x => x.UserName == dto.UserName);


            game.GetPlayerCurrentRound(player.Id).AddAnswers(dto.Answers);

            var playerAnswers = game.GetPlayerCurrentRound(player.Id).Answers;

            var answers = new Dictionary<string, string>();

            await Clients.GroupExcept(dto.GameId.ToString(), Context.ConnectionId).SendAsync("player.answersSended", new
            {
                ok = true,
                answers = playerAnswers
            });
        }

        [HubMethodName("player.sendAnswersValidations")]
        public async Task SendThemeAnswersValidation(SendThemeAnswersValidationDto dto)
        {
            var game = _games[dto.GameId];

            var player = game.Players.FirstOrDefault(x => x.UserName == dto.UserName);

            game.GetPlayerCurrentRound(player.Id).AddThemeAnswersValidations(new ThemeValidation(dto.Validation.Theme, dto.Validation.AnswersValidations));

            await Clients.Caller.SendAsync("player.themeValidationsReceived", new
            {
                ok = true,
                dto.Validation.Theme
            });

            // Se todos os jogadores ja enviaram as validações para esse tema, a pontuação já pode ser processada
            if (game.AllPlayersSendValidationsOfTheme(dto.Validation.Theme))
                game.ProccessPontuationForTheme(dto.Validation.Theme);

            if (game.AllPlayersSendValidationsOfAllThemes())
            {
                await Clients.Group(dto.GameId.ToString()).SendAsync("game.roundFinished", new
                {
                    ok = true
                });

                game.ChangeStatusOfAllPlayersToWait();
                await SendUpdatedScoreboardToAllConnections(dto.GameId, game);
            }
        }

        private async Task SendUpdatedScoreboardToAllConnections(Guid gameId, Game game)
        {
            await Clients.Group(gameId.ToString()).SendAsync("game.updatedScoreboard", new
            {
                ok = true,
                scoreboard = game.CurrentRound.Players.Select(x => new
                {
                    x.Player.UserName,
                    roundPontuation = x.EarnedPoints,
                    gamePontuation = x.Player.EarnedPoints
                })
            });
        }

        [HubMethodName("player.changeStatus")]
        public async Task ChangePlayerStatus(ChangePlayerStatusDto dto)
        {
            var player = GetPlayerInGame(dto.GameId, dto.UserName);

            player.IsReady = dto.IsReady;

            await Clients.GroupExcept(dto.GameId.ToString(), Context.ConnectionId).SendAsync("player.statusChanged", new
            {
                ok = true,
                player
            });
        }

        private Player GetPlayerInGame(Guid gameId, string userName) =>
            _games[gameId].Players.FirstOrDefault(x => x.UserName == userName);
    }

    public class SendThemeAnswersValidationDto
    {
        public Guid GameId { get; set; }
        public string UserName { get; set; }
        public ThemeValidation Validation { get; set; }
    }

    public class SendAnswersDto
    {
        public Guid GameId { get; set; }
        public string UserName { get; set; }
        public ICollection<ThemeAnswer> Answers { get; set; }
    }

    public class StopDto
    {
        public Guid GameId { get; set; }
        public string UserName { get; set; }
    }

    public class ChangePlayerStatusDto
    {
        public Guid GameId { get; set; }
        public string UserName { get; set; }
        public bool IsReady { get; set; }
    }

    public class StartGameDto
    {
        public string UserName { get; set; }
        public Guid GameRoomId { get; set; }
    }

    public class ConnectToGameRoom
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
    }

    public class JoinToGameRoomDto
    {
        public string UserName { get; set; }
        public Guid GameRoomId { get; set; }
    }
}
