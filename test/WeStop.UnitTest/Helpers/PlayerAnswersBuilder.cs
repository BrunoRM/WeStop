using System;
using System.Collections.Generic;
using WeStop.Api.Domain;

namespace WeStop.UnitTest.Helpers
{
    public class PlayerAnswersBuilder
    {
        private Guid _playerId;
        private int _roundNumber;
        private readonly Guid _gameId;
        private ICollection<Answer> _answers;
        
        public PlayerAnswersBuilder(Game game)
        {
            _gameId = game.Id;
            _roundNumber = game.CurrentRoundNumber;
            _answers = new List<Answer>();
        }

        public PlayerAnswersBuilder ForPlayer(User user)
        {
            _playerId = user.Id;
            _answers = new List<Answer>();
            return this;
        }

        public PlayerAnswersBuilder AddAnswer(string theme, string answer)
        {
            _answers.Add(new Answer(theme, answer));
            return this;
        }

        public RoundAnswers Build() =>
            new RoundAnswers(_gameId, _roundNumber, _playerId, _answers);
    }
}