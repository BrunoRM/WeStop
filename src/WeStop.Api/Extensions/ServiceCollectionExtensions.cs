using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using WeStop.Api.Domain.Services;
using WeStop.Api.Infra.Storages.InMemory;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.Api.Infra.Timers;

namespace WeStop.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IThemeStorage, ThemeStorage>();
            services.AddSingleton<IPlayerStorage, PlayerStorage>();
            services.AddSingleton<IGameStorage, GameStorage>();
            services.AddScoped<GameManager, GameManager>();
            services.AddScoped<PlayerManager, PlayerManager>();
            services.AddScoped<RoundScorer, RoundScorer>();
            services.AddScoped<GameTimer, GameTimer>();

            var assembly = AppDomain.CurrentDomain.Load("WeStop.Api");
            services.AddAutoMapper(assembly);

            return services;
        }
    }
}