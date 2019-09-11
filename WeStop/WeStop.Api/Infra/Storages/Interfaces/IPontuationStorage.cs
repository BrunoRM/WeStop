using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Classes;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IPontuationStorage
    {
        Task AddAsync(RoundPontuations pontuations);
        Task<IEnumerable<RoundPontuations>> GetPontuationsAsync(Guid gameId, int roundNumber);
        RoundPontuations GetPlayerPontuationsAsync(Guid gameId, int roundNumber, Guid playerId);
        Task<IEnumerable<RoundPontuations>> GetPontuationsAsync(Guid gameId);
        Task<RoundScoreboard> GetRoundScoreboard(Guid gameId, int roundNumber);
        Task<GameScoreboard> GetGameScoreboard(Guid gameId);
    }
}
