using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Core;

namespace WeStop.UnitTest.Helpers
{
    public class PlayerValidationsBuilder
    {
        private readonly Guid _gameId;
        private readonly int _roundNumber;
        private Guid _playerId;
        private string _theme;
        private ICollection<Validation> _validAnswers = new List<Validation>();
        private ICollection<Validation> _invalidAnswers = new List<Validation>();

        public PlayerValidationsBuilder(Game game)
        {
            _gameId = game.Id;
            _roundNumber = game.CurrentRoundNumber;
        }

        public PlayerValidationsBuilder ForPlayer(User user)
        {
            _playerId = user.Id;
            _validAnswers = new List<Validation>();
            _invalidAnswers = new List<Validation>();
            return this;
        }

        public PlayerValidationsBuilder ForTheme(string theme)
        {
            _theme = theme;
            return this;
        }

        public PlayerValidationsBuilder ValidateAnswers(params string[] values)
        {
            foreach (var value in values)
            {
                var answer = new Answer(_theme, value);
                var validation = new Validation(answer, true);

                _validAnswers.Add(validation);
            }

            return this;
        }

        public PlayerValidationsBuilder InvalidateAnswers(params string[] values)
        {
            foreach (var value in values)
            {
                var answer = new Answer(_theme, value);
                var validation = new Validation(answer, false);

                _invalidAnswers.Add(validation);
            }

            return this;
        }

        public RoundValidations Build()
        {
            var validations = _validAnswers.Union(_invalidAnswers).ToList();
            return new RoundValidations(_gameId, _roundNumber, _playerId, _theme, validations);
        }
    }
}