using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Classes;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IAnswersStorage
    {
        Task AddAsync(RoundAnswers answers);
        Task<RoundAnswers> GetPlayerAnswersInRoundAsync(Guid player, Guid gameId, int roundNumber);
        Task<IEnumerable<RoundAnswers>> GetPlayersAnswersInRound(Guid gameId, int roundNumber);
        Task<string[]> GetDistinctsAnswersForThemeAsync(Guid gameId, int roundNumber, string theme);
        Task<IEnumerable<Guid>> GetPlayersIdsThatRepliedAnswerForThemeInRoundAsync(Guid gameId, int roundNumber, string theme, string answer);
        Task<IEnumerable<Guid>> GetPlayersIdsWithBlankAnswersForThemeInRoundAsync(Guid gameId, int roundNumber, string theme);
    }
}
