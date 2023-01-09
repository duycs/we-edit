using CronJob;
using CrossCutting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz.Logging;
using Serilog;

// Refs:
// https://damienbod.com/2021/11/08/asp-net-core-scheduling-with-quartz-net-and-signalr-monitoring/
// https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/microsoft-di-integration.html#di-aware-job-factories

Console.WriteLine("Cron Jobs running...");

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration;

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory());

var env = builder.Environment.EnvironmentName;
configurationBuilder.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

configuration = configurationBuilder.Build();


// use serilog to console
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
logger.Information($"Start CronJob at environment: {env}");

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.
builder.Services.AddLayersInjector(configuration);

// Inject cron jobs 
// Add logging cron jobs
// Ref: https://damienbod.com/2021/11/08/asp-net-core-scheduling-with-quartz-net-and-signalr-monitoring/
LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
builder.Services.AddCronJobs(configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder.SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapHub<JobsHub>("/jobshub", options =>
{
    options.Transports =
        HttpTransportType.WebSockets |
        HttpTransportType.LongPolling;
}
);

app.MapRazorPages();

app.Run();
