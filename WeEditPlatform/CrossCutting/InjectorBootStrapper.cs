using Application.Services;
using Infrastructure.Pagging;
using Infrastructure.Persistences;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CrossCutting
{
    public static class InjectorBootStrapper
    {
        public static void AddLayersInjector(this IServiceCollection services, IConfiguration configuration)
        {
            // Infrastructure
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
                services.AddDbContext<ProductionContext>(options => options.UseMySQL(connectionString));
            }

            // Persistence: generic repository
            services.AddTransient<IDatabaseService, ProductionContext>();
            services.AddScoped<IRepositoryService, RepositoryService>();

            // Application: service usecase
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<INoteService, NoteService>();


            // Present
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUriService>(o =>
            {
                var request = o.GetRequiredService<IHttpContextAccessor>().HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });

            // Logger
            services.AddSingleton<ILoggerFactory, LoggerFactory>();

            // client services
            services.AddHttpClient<ISSOService>();
            services.AddTransient<ISSOService, SSOService>();

        }
    }
}