using System;
using System.Collections.Concurrent;
using System.Timers;
using Microsoft.AspNetCore.SignalR;
using WeStop.Api.Infra.Hubs;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Classes
{
    public class GameTimerContext
    {
        private readonly IHubContext<GameRoomHub> _hubContext;
        private static ConcurrentDictionary<Guid, Timer> _roundTimers = new ConcurrentDictionary<Guid, Timer>();
        private static ConcurrentDictionary<Guid, Timer> _validationTimers = new ConcurrentDictionary<Guid, Timer>();
        private readonly IGameStorage _gameStorage;

        public GameTimerContext(IHubContext<GameRoomHub> hubContext, IGameStorage gameStorage)
        {
            this._hubContext = hubContext;
            this._gameStorage = gameStorage;
        }

        public async void AddRoundTimer(Guid gameId)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);

            var roundTimer = new Timer(1000);            
            roundTimer.Elapsed += async (sender, args) => 
            {
                game.CurrentRound.RefreshRoundTime();
                await _hubContext.Clients.Group(gameId.ToString()).SendAsync("roundTimeElapsed", game.CurrentRound.CurrentRoundTime);

                if (game.CurrentRound.CurrentRoundTime == game.Options.RoundTime)
                {
                    StopRoundTimer(game.Id);
                    await _hubContext.Clients.Group(gameId.ToString()).SendAsync("players.stopCalled", new
                    {
                        ok = true,
                        reason = "TIME_OVER"
                    });
                }
            };

            _roundTimers.AddOrUpdate(gameId, roundTimer, (id, Timer) => 
            {
                return Timer;
            });
        }

        public void StartRoundTimer(Guid gameId)
        {
            _roundTimers[gameId].Enabled = true;
            _roundTimers[gameId].Start();
        }

        public void StopRoundTimer(Guid gameId) =>
            _roundTimers[gameId].Stop();

        public async void AddValidationTimer(Guid gameId)
        {
            var game = await _gameStorage.GetByIdAsync(gameId);

            var validationTimer = new Timer(1000);
            validationTimer.Elapsed += async (sender, args) => 
            {
                game.CurrentRound.RefreshValidationTime();
                await _hubContext.Clients.Group(gameId.ToString()).SendAsync("validationTimeElapsed", game.CurrentRound.CurrentValidationTime);

                if (game.CurrentRound.CurrentValidationTime == game.Options.RoundTime)
                {
                    validationTimer.Stop();
                    await _hubContext.Clients.Group(game.Id.ToString()).SendAsync("validationTimeOver");
                }
            };

            _validationTimers.AddOrUpdate(gameId, validationTimer, (id, Timer) => 
            {
                return Timer;
            });
        }

        public void StartValidationTimer(Guid gameId)
        {
            _validationTimers[gameId].Enabled = true;
            _validationTimers[gameId].Start();
        }

        public void StopValidationTimer(Guid gameId) =>
            _validationTimers[gameId].Stop();
    }
}