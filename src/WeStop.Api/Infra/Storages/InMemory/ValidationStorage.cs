using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages.InMemory
{
    public class ValidationStorage : IValidationStorage
    {
        private static readonly ICollection<RoundValidations> _roundValidations = new List<RoundValidations>();

        public Task AddAsync(RoundValidations validations)
        {
            _roundValidations.Add(validations);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<RoundValidations>> GetValidationsAsync(Guid gameId, int roundNumber)
        {
            return Task.FromResult<IEnumerable<RoundValidations>>(
                _roundValidations.Where(rv => rv.GameId == gameId && rv.RoundNumber == roundNumber).ToList());
        }

        public Task<IEnumerable<RoundValidations>> GetValidationsAsync(Guid gameId, int roundNumber, Guid playerId)
        {
            return Task.FromResult<IEnumerable<RoundValidations>>(
                _roundValidations.Where(rv => rv.GameId == gameId && rv.RoundNumber == roundNumber && rv.PlayerId == playerId).ToList());
        }

        public Task<RoundValidations> GetValidationsAsync(Guid gameId, int roundNumber, Guid playerId, string theme)
        {
            return Task.FromResult<RoundValidations>(
                _roundValidations.FirstOrDefault(rv => rv.GameId == gameId && rv.RoundNumber == roundNumber && rv.PlayerId == playerId && rv.Theme.Equals(theme)));
        }

        public Task<IEnumerable<RoundValidations>> GetValidationsAsync(Guid gameId, int roundNumber, string theme)
        {
            return Task.FromResult<IEnumerable<RoundValidations>>(
                _roundValidations.Where(rv => rv.GameId == gameId && rv.RoundNumber == roundNumber && rv.Theme.Equals(theme)).ToList());
        }

        public Task<bool> HasPlayerValidationsInRound(Guid gameId, int roundNumber, Guid playerId)
        {
            return Task.FromResult(
                _roundValidations.Any(x => x.GameId == gameId && x.RoundNumber == roundNumber && x.PlayerId == playerId));
        }
    }
}