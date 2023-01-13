using Application.Models;
using Application.Operations.Actions.AssignAction;
using Application.Services;
using Infrastructure.Commands;
using Infrastructure.Events;
using Infrastructure.Models;
using Infrastructure.Pagging;
using Infrastructure.Persistences;
using Infrastructure.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
                services.AddDbContext<ProductionContext>(options => options.UseMySQL(connectionString,
                    options => options.CommandTimeout(300)));
            }

            // Persistence: generic repository
            services.AddTransient<IDatabaseService, ProductionContext>();
            services.AddScoped<IRepositoryService, RepositoryService>();

            // Command
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddMediatR(typeof(AssignActionCommandHandler).GetTypeInfo().Assembly);
            services.AddScoped<IRequestHandler<AssignActionCommand, InvokeResult>, AssignActionCommandHandler>();

            // Application: service usecase
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<INoteService, NoteService>();
            services.AddScoped<IOperationService, OperationService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IFlowService, FlowService>();
            services.AddScoped<IRouteService, RouteService>();

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