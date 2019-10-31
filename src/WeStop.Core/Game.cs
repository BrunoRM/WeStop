using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Core.Exceptions;
using WeStop.Core.Extensions;

namespace WeStop.Core
{
    public sealed class Game
    {
        public Game(string name, string password, GameOptions options)
        {
            Id = Guid.NewGuid();
            Name = name;
            Password = password;
            Options = options;
            State = GameState.Waiting;
            Rounds = new List<Round>();
            Players = new List<Player>();
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Password { get; private set; }
        public GameOptions Options { get; private set; }
        public GameState State { get; private set; }
        public Round PreviousRound => Rounds.SkipLast(1).LastOrDefault();
        public int PreviousRoundNumber => PreviousRound?.Number ?? 1;
        public Round CurrentRound => Rounds.LastOrDefault();
        public int NextRoundNumber => Rounds.Count == 1 ? 1 : CurrentRound?.Number + 1 ?? 1;
        public int CurrentRoundNumber => CurrentRound?.Number ?? 0;
        public ICollection<Round> Rounds { get; private set; }
        public ICollection<Player> Players { get; set; }
        public ICollection<Guid> AuthorizedPlayersIds { get; set; }

        public bool IsPrivate() =>
            !string.IsNullOrEmpty(Password);

        public Round StartNextRound()
        {
            if (IsFinalRound())
                throw new WeStopException("O jogo já chegou ao fim. Não é possível iniciar a rodada");
            
            var newRound = CreateNewRound();
            Rounds.Add(newRound);
            State = GameState.InProgress;
            return newRound;
        }

        private Round CreateNewRound()
        {
            int nextRoundNumber = CurrentRound?.Number + 1 ?? 1;
            string drawnLetter = SortOutLetter();
            var round = new Round(this.Id, nextRoundNumber, drawnLetter);
            return round;
        }

        private string SortOutLetter()
        {
            string[] notSortedLetters = GetNotSortedLetters();
            int randomSortedIndex = new Random().Next(notSortedLetters.Count()-1);
            string sortedLetter = notSortedLetters[randomSortedIndex];

            Options.AvailableLetters[sortedLetter] = true;
            return sortedLetter;
        }

        private string[] GetNotSortedLetters() =>
            Options.AvailableLetters.Where(al => !al.Value).Select(al => al.Key).ToArray();

        public IReadOnlyCollection<PlayerPontuation> GetScoreboard(int roundNumber)
        {
            var playersPontuations = new List<PlayerPontuation>();
            foreach (var player in Players)
            {
                var roundPontuations = player.Pontuations.GetRoundPontuations(roundNumber);
                if (roundPontuations != null)
                {
                    playersPontuations.Add(new PlayerPontuation
                    {
                        PlayerId = player.Id,
                        UserName = player.UserName,
                        RoundPontuation = roundPontuations.TotalPontuation,
                        GamePontuation = player.TotalPontuation,
                        Pontuations = roundPontuations.ThemesPontuations
                    });
                }
            }

            return playersPontuations.ToList();
        }

        public bool IsFinalRound() =>
            CurrentRound?.Number == Options.Rounds;

        public void StartValidations() =>
            State = GameState.Validations;

        public void FinishRound() =>
            State = GameState.Waiting;

        public void Finish()
        {
            if (!IsFinalRound())
                throw new WeStopException("O jogo só poderá ser finalizado se a rodada atual for a última");

            State = GameState.Finished;
        }

        public IEnumerable<string> GetWinners()
        {
            if (!IsFinalRound())
                throw new WeStopException("O jogo ainda não chegou ao fim");

            var highPontuation = Players.GetHighPontuation();
            var gameWinners = Players.GetPlayersWithPontuation(highPontuation);

            foreach (var player in gameWinners)
            {
                yield return Players.First(p => p.Id == player.Id).UserName;
            }
        }

        public string[] GetAvailableLetters() =>
            Options.AvailableLetters.Where(x => !x.Value).Select(x => x.Key).ToArray();

        public string[] GetSortedLetters() =>
            Options.AvailableLetters.Where(x => x.Value).Select(x => x.Key).ToArray();

        public bool IsFull() =>
            Players.Count == Options.NumberOfPlayers;

        public bool HasPlayer(Guid playerId) =>
            Players.Any(p => p.Id == playerId);

        public Player GetPlayer(Guid playerId) =>
            Players.FirstOrDefault(p => p.Id == playerId);

        public void AuthorizePlayer(Guid playerId)
        {
            if (!IsPlayerAuthorized(playerId))
            {
                AuthorizedPlayersIds.Add(playerId);
            }
        }

        public void UnauthorizePlayer(Guid playerId)
        {
            if (IsPlayerAuthorized(playerId))
            {
                AuthorizedPlayersIds.Remove(playerId);
            }
        }

        public bool IsValidForJoin(string passsword, out string status)
        {
            if (IsFinalRound())
            {
                status = "GAME_IN_FINAL_ROUND";
                return false;
            }

            if (IsFull())
            {
                status = "GAME_FULL";
                return false;
            }

            if (IsPrivate())
            {
                if (string.IsNullOrEmpty(passsword))
                {
                    status = "PASSWORD_REQUIRED";
                    return false;
                }

                if (!Password.Equals(passsword))
                {
                    status = "INCORRECT_PASSWORD";
                    return false;
                }
            }

            status = "OK";
            return true;
        }

        public bool IsPlayerAuthorized(Guid playerId) =>
            AuthorizedPlayersIds.Contains(playerId);
    }
}