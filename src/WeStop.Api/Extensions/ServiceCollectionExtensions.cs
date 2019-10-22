using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using WeStop.Api.Core.Services;
using WeStop.Api.Infra.Timers;

namespace WeStop.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
        {
            //services.AddInMemoryStorages();
            services.AddMongoStorages();
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