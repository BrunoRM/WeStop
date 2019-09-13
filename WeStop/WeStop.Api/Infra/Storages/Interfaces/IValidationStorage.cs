using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Domain;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IValidationStorage
    {
        Task AddAsync(RoundValidations validations);
        Task<IEnumerable<RoundValidations>> GetValidationsAsync(Guid gameId, int roundNumber);
        Task<bool> HasPlayerValidationsInRound(Guid gameId, int roundNumber);
    }
}
