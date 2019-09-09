using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Extensions;

namespace WeStop.Api.Classes
{
    public class Player
    {
        private ICollection<RoundAnswers> _roundsAnswers;
        private ICollection<RoundValidations> _roundsValidations;
        private ICollection<RoundPontuations> _roundsPontuations;

        public Player(Guid gameId, User user, bool isAdmin)
        {
            GameId = gameId;
            User = user;
            IsReady = false;
            IsAdmin = isAdmin;
            _roundsAnswers = new List<RoundAnswers>();
            _roundsValidations = new List<RoundValidations>();
            _roundsPontuations = new List<RoundPontuations>();
        }

        public Guid GameId { get; set; }
        public User User { get; private set; }
        public Guid Id => User.Id;
        public string UserName => User.UserName;
        public bool IsAdmin { get; private set; }
        public IReadOnlyCollection<RoundAnswers> RoundsAnswers => _roundsAnswers.ToList();
        public IReadOnlyCollection<RoundValidations> RoundsValidations => _roundsValidations.ToList();
        public IReadOnlyCollection<RoundPontuations> RoundsPontuations => _roundsPontuations.ToList();
        public bool IsReady { get; private set; }
        public int EarnedPoints => RoundsPontuations.Sum(rp => rp.ThemesPontuations.Sum(tp => tp.Pontuation));

        public void AddAnswersForRound(Guid roundId, ICollection<ThemeAnswer> answers)
        {
            foreach (var themeAnswer in answers)
            {
                _roundsAnswers.AddThemeAnswerForRound(roundId, themeAnswer.Theme, themeAnswer.Answer);
            }
        }

        public void AddRoundValidationsForTheme(Guid roundId, ICollection<ThemeValidation> roundValidations)
        {
            if (!RoundsValidations.Any(rv => rv.RoundId == roundId))
                _roundsValidations.Add(new RoundValidations(roundId, roundValidations));
        }

        public void AddPontuationForThemeInRound(Guid roundId, string theme, int points)
        {
            _roundsPontuations.AddPontuationForThemeInRound(roundId, theme, points);
        }

        // Get pontuation in a round
        public int this[Guid roundId]
        {
            get =>
                RoundsPontuations.FirstOrDefault(rp => rp.RoundId == roundId)?.GetTotalPontuation() ?? 0;
        }

        // Get pontuation for specific theme in a round
        public int this[Guid roundId, string theme]
        {
            get =>
                RoundsPontuations.FirstOrDefault(rp => rp.RoundId == roundId)?.GetPontuationForTheme(theme) ?? 0;
        }

        public void ChangeStatus(bool isReady)
        {
            IsReady = isReady;
        }
    }
}