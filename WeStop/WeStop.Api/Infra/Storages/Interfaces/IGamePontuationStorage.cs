using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Classes;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IGamePontuationStorage
    {
        Task AddAsync(RoundPontuations pontuations);
        Task<IEnumerable<RoundPontuations>> GetPlayersPontuationsForRoundAsync(Guid gameId, int roundNumber);
        RoundPontuations GetPlayerPontuationsForRoundAsync(Guid gameId, int roundNumber, Guid playerId);
    }
}
