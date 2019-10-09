using System;
using System.Threading.Tasks;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Domain.Services
{
    public sealed class PlayerManager
    {
        private readonly IPlayerStorage _playerStorage;

        public PlayerManager(IPlayerStorage playerStorage)
        {
            _playerStorage = playerStorage;
        }

        public async Task<Player> ChangeStatusAsync(Guid gameId, Guid playerId, bool isReady)
        {
            var player = await _playerStorage.GetAsync(gameId, playerId);
            player.IsReady = isReady;

            await _playerStorage.EditAsync(player);
            return player;
        }
    }
}
