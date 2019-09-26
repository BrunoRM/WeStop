﻿using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using WeStop.Api.Domain.Services;
using WeStop.Api.Infra.Storages.InMemory;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.Api.Infra.Timers;
using WeStop.Api.Managers;

namespace WeStop.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IAnswerStorage, AnswerStorage>();
            services.AddSingleton<IPontuationStorage, PontuationStorage>();
            services.AddSingleton<IValidationStorage, ValidationStorage>();
            services.AddSingleton<IThemeStorage, ThemeStorage>();
            services.AddSingleton<IUserStorage, UserStorage>();
            services.AddSingleton<IPlayerStorage, PlayerStorage>();
            services.AddSingleton<IGameStorage, GameStorage>();
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