using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeStop.Core.Storages;
using WeStop.Storage.MongoDb;

namespace WeStop.Api.Extensions
{
    public static class MongoStorageConfig
    {
        public static IServiceCollection AddMongoStorages(this IServiceCollection services,
            IConfiguration configuration)
        {
            var mongodbConnectionString = configuration.GetConnectionString("mongoDb");
            services.AddSingleton<MongoContext, MongoContext>(config => new MongoContext(mongodbConnectionString));
            services.AddSingleton<IThemeStorage, ThemeStorage>();            
            services.AddSingleton<IPlayerStorage, PlayerStorage>();
            services.AddSingleton<IGameStorage, GameStorage>();
            return services;
        }
    }
}