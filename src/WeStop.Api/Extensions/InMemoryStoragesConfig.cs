using Microsoft.Extensions.DependencyInjection;
using WeStop.Api.Infra.Storages.InMemory;
using WeStop.Api.Infra.Storages.Interfaces;

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