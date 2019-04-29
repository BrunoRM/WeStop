using Microsoft.Extensions.DependencyInjection;
using WeStop.Domain.Repositories;
using WeStop.Infra;
using WeStop.Infra.Repositories;

namespace WeStop.Api.Extensions
{
    public static class ConfigureServicesExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<WeStopDbContext, WeStopDbContext>();
            services.AddTransient<IPlayerRepository, PlayerRepository>();
        }
    }
}
