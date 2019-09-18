using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using WeStop.Api.Infra.Hubs;
using WeStop.Api.Infra.Storages.InMemory;
using WeStop.Api.Infra.Storages.Interfaces;
using WeStop.Api.Infra.Timers.Default;
using WeStop.Api.Infra.Timers.Interfaces;
using WeStop.Api.Managers;

namespace WeStop.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddCors(options => options.AddPolicy("WeStopCorsPolicy",
                builder =>
                {
                    builder.WithOrigins("http://localhost:5001", "http://localhost:5002").AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }));

            services.AddSingleton<IAnswerStorage, AnswerStorage>();
            services.AddSingleton<IPontuationStorage, PontuationStorage>();
            services.AddSingleton<IValidationStorage, ValidationStorage>();
            services.AddSingleton<GameManager, GameManager>();
            services.AddSingleton<RoundScorerManager, RoundScorerManager>();
            services.AddSingleton<IThemeStorage, ThemeStorage>();
            services.AddSingleton<IUserStorage, UserStorage>();
            services.AddSingleton<IGameStorage, GameStorage>();
            services.AddScoped<IGameTimer, GameTimer>();

            var assembly = AppDomain.CurrentDomain.Load("WeStop.Api");
            services.AddAutoMapper(assembly);

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("WeStopCorsPolicy");

            app.UseSignalR(routeConfig =>
            {
                routeConfig.MapHub<GameHub>("/game");
            });

            app.UseMvc();
        }
    }
}
