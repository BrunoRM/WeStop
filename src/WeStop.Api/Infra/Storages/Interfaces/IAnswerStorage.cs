using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Domain;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IAnswerStorage
    {
        Task AddAsync(RoundAnswers answers);
        Task<RoundAnswers> GetPlayerAnswersAsync(Guid player, Guid gameId, int roundNumber);
        Task<IEnumerable<RoundAnswers>> GetPlayersAnswersAsync(Guid gameId, int roundNumber);
    }
}
