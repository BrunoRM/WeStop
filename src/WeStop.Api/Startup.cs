﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WeStop.Api.Extensions;
using WeStop.Api.Infra.Hubs;

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
                    builder.WithOrigins("http://localhost:5001", "https://localhost:5001", "http://localhost:5002", "https://localhost:5002").AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }));

            services.ConfigureDependencies();

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
