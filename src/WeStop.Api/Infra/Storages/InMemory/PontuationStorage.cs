using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages.InMemory
{
    public class PontuationStorage : IPontuationStorage
    {
        private static readonly ICollection<RoundPontuations> _pontuations = new List<RoundPontuations>();

        public Task AddAsync(RoundPontuations pontuations)
        {
            _pontuations.Add(pontuations);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<RoundPontuations>> GetPontuationsAsync(Guid gameId, int roundNumber)
        {
            return Task.FromResult(
                _pontuations.Where(p => p.GameId == gameId && p.RoundNumber == roundNumber)
            );
        }

        public Task<RoundPontuations> GetPontuationsAsync(Guid gameId, int roundNumber, Guid playerId)
        {
            return Task.FromResult(
                _pontuations.FirstOrDefault(p => p.GameId == gameId && p.RoundNumber == roundNumber && p.PlayerId == playerId)
            );
        }

        public Task<IEnumerable<RoundPontuations>> GetPontuationsAsync(Guid gameId)
        {
            throw new NotImplementedException();
        }
    }
}