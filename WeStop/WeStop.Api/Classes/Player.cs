using System;
using System.Collections.Generic;
using System.Linq;

namespace WeStop.Api.Classes
{
    public class Player
    {
        public Player(User user, bool isAdmin)
        {
            User = user;
            IsReady = false;
            IsAdmin = isAdmin;
            RoundsAnswers = new List<RoundAnswers>();
            RoundsValidations = new List<RoundValidations>();
            RoundsPontuations = new List<RoundPontuation>();
        }

        public User User { get; private set; }
        public Guid Id => User.Id;
        public string UserName => User.UserName;
        public bool IsAdmin { get; private set; }
        public ICollection<RoundAnswers> RoundsAnswers { get; set; }
        public ICollection<RoundValidations> RoundsValidations { get; set; }
        public ICollection<RoundPontuation> RoundsPontuations { get; set; }
        public bool IsReady { get; private set; }
        public int EarnedPoints => RoundsPontuations.Sum(rp => rp.ThemesPontuations.Sum(tp => tp.Pontuation));

        public void AddRoundAnswers(Guid roundId, ICollection<ThemeAnswer> answers)
        {
            if (!RoundsAnswers.Any(ra => ra.RoundId == roundId))
            {
                RoundsAnswers.Add(new RoundAnswers(roundId, answers));
            }
        }

        public void AddRoundValidationsForTheme(Guid roundId, ICollection<ThemeValidation> roundValidations)
        {
            if (!RoundsValidations.Any(rv => rv.RoundId == roundId))
                RoundsValidations.Add(new RoundValidations(roundId, roundValidations));
        }

        public void AddRoundPontuationForTheme(Guid roundId, ICollection<ThemePontuation> roundPontuations)
        {
            if (!RoundsPontuations.Any(rp => rp.RoundId == roundId))
            {
                RoundsPontuations.Add(new RoundPontuation(roundId, roundPontuations));
            }
        }

        public int GetTotalPontuationForRound(Guid roundId) =>
            RoundsPontuations.FirstOrDefault(rp => rp.RoundId == roundId)?.GetTotalPontuation() ?? 0;

        public void ChangeStatus(bool isReady)
        {
            IsReady = isReady;
        }
    }
}