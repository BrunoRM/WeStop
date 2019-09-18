using System;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Managers
{
    public sealed class PlayerManager
    {
        private readonly IPlayerStorage _playerStorage;

        public PlayerManager(IPlayerStorage playerStorage)
        {
            _playerStorage = playerStorage;
        }

        public async Task ChangeStatusAsync(Guid gameId, Guid playerId, bool isReady, Action<Player> action)
        {
            var player = await _playerStorage.GetAsync(gameId, playerId);
            player.ChangeStatus(isReady);

            await _playerStorage.UpdateAsync(player);
            action?.Invoke(player);
        }
    }
}
