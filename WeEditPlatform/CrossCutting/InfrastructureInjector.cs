using Infrastructure.Persistences;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting
{
    public static class InfrastructureInjector
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DataAccess in memory or relation database
            var useInMemoryDb = configuration.GetValue<bool>("UseInMemoryDb");
            var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS_WEEDIT");
            if (useInMemoryDb)
            {
                services.AddDbContext<ProductionContext>(options => options.UseInMemoryDatabase(configuration.GetConnectionString("Default")));
            }
            else
            {
                // Mysql provider
                services.AddDbContext<ProductionContext>(options => options.UseMySQL(connectionString,
                    options => options.CommandTimeout(300)));
            }

            // Persistence: generic repository
            services.AddTransient<IDatabaseService, ProductionContext>();
            services.AddScoped<IRepositoryService, RepositoryService>();
        }
    }
}
