using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Exceptions;

namespace WeStop.Api.Domain
{
    public sealed class Game
    {
        public Game(string name, string password, GameOptions options)
        {
            Id = Guid.NewGuid();
            Name = name;
            Password = password;
            Options = options;
            Rounds = new List<Round>();
            State = GameState.Waiting;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Password { get; private set; }
        public GameOptions Options { get; private set; }
        public GameState State { get; private set; }
        public int PreviousRoundNumber => Rounds.Last()?.Number - 1 ?? 1;
        public int CurrentRoundNumber => Rounds.Last()?.Number ?? 1;
        public Round CurrentRound => Rounds.First(r => r.Number == CurrentRoundNumber);
        public ICollection<Round> Rounds { get; private set; }

        public void StartNextRound()
        {
            if (IsFinalRound())
                throw new WeStopException("O jogo já chegou ao fim. Não é possível iniciar a rodada");

            var newRound = CreateNewRound();
            Rounds.Add(newRound);
            State = GameState.InProgress;
        }

        private Round CreateNewRound()
        {
            int nextRoundNumber = CurrentRoundNumber;
            string drawnLetter = SortOutLetter();
            var newRound = new Round(nextRoundNumber, drawnLetter);

            Rounds.Add(newRound);
            return newRound;
        }

        private string SortOutLetter()
        {
            string[] notSortedLetters = GetNotSortedLetters();
            int randomSortedIndex = new Random().Next(notSortedLetters.Count());

            string sortedLetter = notSortedLetters[randomSortedIndex];

            Options.AvailableLetters[sortedLetter] = true;
            return sortedLetter;
        }

        private string[] GetNotSortedLetters() =>
            Options.AvailableLetters.Where(al => al.Value == false).Select(al => al.Key).ToArray();

        public bool IsFinalRound() =>
            CurrentRound?.Number == Options.Rounds;

        public void BeginCurrentRoundThemesValidations() =>
            State = GameState.ThemesValidations;

        public void FinishRound() =>
            State = GameState.Waiting;

        public void Finish()
        {
            if (!IsFinalRound())
                throw new WeStopException("O jogo só poderá ser finalizado se a rodada atual for a última");

            State = GameState.Finished;
        }        
    }
}