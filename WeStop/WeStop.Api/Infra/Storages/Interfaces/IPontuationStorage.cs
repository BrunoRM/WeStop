using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Domain;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IPontuationStorage
    {
        Task AddAsync(RoundPontuations pontuations);
        Task<IEnumerable<RoundPontuations>> GetPontuationsAsync(Guid gameId, int roundNumber);
        RoundPontuations GetPontuationsAsync(Guid gameId, int roundNumber, Guid playerId);
        Task<IEnumerable<RoundPontuations>> GetPontuationsAsync(Guid gameId);
    }
}
