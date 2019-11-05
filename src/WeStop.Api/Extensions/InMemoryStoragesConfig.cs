using Microsoft.Extensions.DependencyInjection;
using WeStop.Core.Storages;
using WeStop.Storage.InMemory;

namespace WeStop.Api.Extensions
{
    public static class InMemoryStoragesConfig
    {
        public static IServiceCollection AddInMemoryStorages(this IServiceCollection services)
        {
            services.AddSingleton<IThemeStorage, ThemeStorage>();
            services.AddSingleton<IPlayerStorage, PlayerStorage>();
            services.AddSingleton<IGameStorage, GameStorage>();
            return services;
        }
    }
}