using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages.InMemory
{
    public class AnswerStorage : IAnswerStorage
    {
        private static readonly ICollection<RoundAnswers> _answers = new List<RoundAnswers>();

        public Task AddAsync(RoundAnswers answers)
        {
            _answers.Add(answers);
            return Task.CompletedTask;
        }

        public Task<RoundAnswers> GetPlayerAnswersAsync(Guid player, Guid gameId, int roundNumber)
        {
            return Task.FromResult<RoundAnswers>(
                _answers.FirstOrDefault(a => a.GameId == gameId && a.RoundNumber == roundNumber && a.PlayerId == player));
        }

        public Task<IEnumerable<RoundAnswers>> GetPlayersAnswersAsync(Guid gameId, int roundNumber)
        {
            return Task.FromResult<IEnumerable<RoundAnswers>>(
                _answers.Where(a => a.GameId == gameId && a.RoundNumber == roundNumber).ToList());
        }
    }
}