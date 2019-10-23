using Microsoft.Extensions.DependencyInjection;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.Api.Infra.Storages.MongoDb;

namespace WeStop.Api.Extensions
{
    public static class MongoStorageConfig
    {
        public static IServiceCollection AddMongoStorages(this IServiceCollection services)
        {
            services.AddSingleton<MongoContext, MongoContext>(config => new MongoContext("mongodb://127.0.0.1:27017/admin"));
            services.AddSingleton<IThemeStorage, ThemeStorage>();            
            services.AddSingleton<IPlayerStorage, PlayerStorage>();
            services.AddSingleton<IGameStorage, GameStorage>();
            return services;
        }
    }
}