using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Core
{
    public sealed class Player
    {
        private Player(Guid gameId, User user, bool isAdmin)
        {
            GameId = gameId;
            User = user;
            IsReady = false;
            IsAdmin = isAdmin;
            IsOnline = true;
            InRound = false;
            Answers = new List<RoundAnswers>();
            Validations = new List<RoundValidations>();
            Pontuations = new List<RoundPontuations>();
        }

        public static Player CreateAsAdmin(Guid gameId, User user) =>
            new Player(gameId, user, true);

        public static Player Create(Guid gameId, User user) =>
            new Player(gameId, user, false);

        public Guid GameId { get; private set; }
        public User User { get; set; }
        public Guid Id => User.Id;
        public string UserName => User.UserName;
        public bool IsAdmin { get; private set; }
        public bool IsReady { get; set; }
        public bool IsOnline { get; private set; }
        public bool InRound { get; set; }
        public ICollection<RoundAnswers> Answers { get; set; }
        public ICollection<RoundValidations> Validations { get; set; }
        public ICollection<RoundPontuations> Pontuations { get; set; }
        public int TotalPontuation => Pontuations.Sum(p => p.TotalPontuation);

        public bool HasValidatedTheme(int roundNumber, string theme) =>
            Validations.Any(v => v.RoundNumber == roundNumber && v.Theme.Equals(theme));

        public RoundAnswers GetAnswersInRound(int roundNumber) =>
            Answers.FirstOrDefault(a => a.RoundNumber == roundNumber);

        public RoundValidations GetValidationsInRound(int roundNumber) =>
            Validations.FirstOrDefault(v => v.RoundNumber == roundNumber);

        public RoundPontuations GetPontuationInRound(int roundNumber) =>
            Pontuations.FirstOrDefault(p => p.RoundNumber == roundNumber);

        public int GetTotalValidationsInRound(int roundNumber) =>
            Validations.Where(v => v.RoundNumber == roundNumber).Count();
    }
}