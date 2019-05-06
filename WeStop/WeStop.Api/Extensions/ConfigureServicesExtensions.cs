using Microsoft.Extensions.DependencyInjection;
using WeStop.Infra;

namespace WeStop.Api.Extensions
{
    public static class ConfigureServicesExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<WeStopDbContext, WeStopDbContext>();
        }
    }
}
