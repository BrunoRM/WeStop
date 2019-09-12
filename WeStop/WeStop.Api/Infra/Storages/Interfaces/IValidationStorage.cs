using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Classes;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IValidationStorage
    {
        Task AddAsync(RoundValidations validations);
        Task<IEnumerable<RoundValidations>> GetValidationsAsync(Guid gameId, int roundNumber);
        Task<bool> HasPlayerValidatedThemeAsync(Guid gameId, int roundNumber, string theme);
    }
}
