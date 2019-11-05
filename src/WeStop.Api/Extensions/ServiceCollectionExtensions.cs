using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WeStop.Api.Infra.Timers;
using WeStop.Core.Services;

namespace WeStop.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            //services.AddInMemoryStorages();
            services.AddMongoStorages(configuration);
            services.AddSingleton<GameManager, GameManager>();
            services.AddSingleton<PlayerManager, PlayerManager>();
            services.AddSingleton<RoundScorer, RoundScorer>();
            services.AddSingleton<GameTimer, GameTimer>();

            var assembly = AppDomain.CurrentDomain.Load("WeStop.Api");
            services.AddAutoMapper(assembly);

            return services;
        }
    }
}