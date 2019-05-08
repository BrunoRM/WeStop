using Microsoft.Extensions.DependencyInjection;
using WeStop.Infra;
using WeStop.Infra.Database;

namespace WeStop.Api.Extensions
{
    public static class ConfigureServicesExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<WeStopDbContext, WeStopSqlServerDbContext>();
        }
    }
}
