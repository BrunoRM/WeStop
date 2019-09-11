using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Classes;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IValidationStorage
    {
        Task AddAsync(RoundValidations validations);
        Task<IEnumerable<RoundValidations>> GetRoundValidationsAsync(Guid gameId, int roundNumber);
        Task<int> GetValidVotesCountForAnswerAsync(Guid gameId, int roundNumber, Answer answer);
        Task<int> GetInvalidVotesCountForAnswerAsync(Guid gameId, int roundNumber, Answer answer);
        Task<bool> HasPlayerValidatedTheme(Guid gameId, int roundNumber, string theme);
    }
}
