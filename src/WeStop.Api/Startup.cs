using Microsoft.AspNetCore.Builder;
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
            var allowedCORSOrigins = Configuration.GetSection("AllowedCORS").Get<string[]>();
            services.AddCors(options => options.AddPolicy("WeStopCorsPolicy",
                builder =>
                {
                    builder.WithOrigins(allowedCORSOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }));

            services.AddSignalR();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
                
            services.ConfigureDependencies(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("WeStopCorsPolicy");
            app.UseWebSockets();
            app.UseSignalR(routeConfig =>
            {
                routeConfig.MapHub<GameHub>("/game");
                routeConfig.MapHub<LobbyHub>("/lobby");
            });

            app.UseMvc();
        }
    }
}
