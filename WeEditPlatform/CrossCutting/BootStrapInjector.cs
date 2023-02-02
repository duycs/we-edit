using Infrastructure.Persistences;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CrossCutting
{
    public static class BootStrapInjector
    {
        public static void AddLayersInjector(this IServiceCollection services, IConfiguration configuration)
        {
            // Infrastructure
            services.AddInfrastructure(configuration);

            // Application: service usecase, client service, command handle
            services.AddServices();

            // Logger
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
        }
    }
}