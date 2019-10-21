﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeStop.Api.Core;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IPlayerStorage
    {
        Task AddAsync(Player player);
        Task EditAsync(Player player);
        Task<Player> GetAsync(Guid gameId, Guid playerId);
        Task<ICollection<Player>> GetPlayersInRoundAsync(Guid gameId);
        Task<ICollection<Player>> GetAllAsync(Guid gameId);
    }
}
