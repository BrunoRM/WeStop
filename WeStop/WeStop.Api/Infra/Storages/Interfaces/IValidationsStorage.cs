using System;
using System.Threading.Tasks;
using WeStop.Api.Classes;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IValidationsStorage
    {
        Task AddAsync(RoundValidations validations);
        Task<int> GetValidVotesCountForThemeAnswerAsync(Guid gameId, int roundNumber, string theme, string answer);
        Task<int> GetInvalidVotesCountForThemeAnswerAsync(Guid gameId, int roundNumber, string theme, string answer);
    }
}
