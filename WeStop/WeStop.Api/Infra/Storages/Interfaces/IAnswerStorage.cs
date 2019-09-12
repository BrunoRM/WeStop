using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Classes;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IAnswerStorage
    {
        Task AddAsync(RoundAnswers answers);
        Task<RoundAnswers> GetPlayerAnswersAsync(Guid player, Guid gameId, int roundNumber);
        Task<IEnumerable<RoundAnswers>> GetPlayersAnswersAsync(Guid gameId, int roundNumber);
        Task<IEnumerable<Answer>> GetDistinctsAnswersAsync(Guid gameId, int roundNumber, string theme);
        Task<IEnumerable<Guid>> GetPlayersIdsThatRepliedAnswerAsync(Guid gameId, int roundNumber, Answer answer);
        Task<IEnumerable<Guid>> GetPlayersIdsWithBlankAnswersAsync(Guid gameId, int roundNumber, string theme);
    }
}
