using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Exceptions;
using WeStop.Api.Extensions;

namespace WeStop.Api.Domain
{
    public sealed class Game
    {
        private readonly ICollection<Player> _players;
        private readonly ICollection<Round> _rounds;
        private GameState _currentState;

        public Game(User user, string name, string password, GameOptions options)
        {
            Id = Guid.NewGuid();
            Name = name;
            Password = password;
            Options = options;
            _players = new List<Player>()
            {
                new Player(this.Id, user, true)
            };
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

        public int GetPlayersCount() =>
            Players.Count();

        public bool IsPlayerAdmin(Guid playerId) =>
            Players.FirstOrDefault(x => x.Id == playerId).IsAdmin;

        public bool HasSuficientPlayersToStartNewRound() =>
            GetPlayersCount() >= 2;

        public int GetNextRoundNumber() =>
            CurrentRound?.Number + 1 ?? 1;

        public int GetCurrentRoundNumber() =>
            CurrentRound?.Number ?? 1;

        public Player AddPlayer(User user)
        {
            var player = _players.FirstOrDefault(p => p.Id == user.Id);
            if (player is null)
            {
                player = new Player(Id, user, false);
                _players.Add(player);
            }

            return player;
        }

        public Player GetPlayer(Guid id) =>
            _players.FirstOrDefault(p => p.Id == id);        

        public void StartNextRound()
        {
            if (IsFinalRound())
                throw new WeStopException("O jogo já chegou ao fim. Não é possível iniciar a rodada");

            CurrentRound = CreateNewRound();
            _currentState = GameState.InProgress;
            SetPlayersInRound();
        }

        private void SetPlayersInRound()
        {
            foreach (var player in Players)
            {
                player.IsInRound = true;
            }
        }

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

        public string[] GetThemes() =>
            Options.Themes;

        public bool IsFinalRound() =>
            CurrentRound?.Number == Options.Rounds;

        public string GetCurrentRoundSortedLetter() =>
            CurrentRound.SortedLetter;

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
    }
}