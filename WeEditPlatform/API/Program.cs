using Application.Services;
using CrossCutting;
using Infrastructure.Persistences;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration;

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory());

// system env
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS_WEEDIT");

var envApp = builder.Environment.EnvironmentName;

builder.Environment.EnvironmentName = env;

configurationBuilder.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

configuration = configurationBuilder.Build();

// use serilog to console
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

logger.Information("ENVIRONMENT");
logger.Information(env);
logger.Information(connectionString);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.
builder.Services.AddLayersInjector(configuration);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// author
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.Authority = configuration.GetValue<string>("AppSettings:ssoUrl");
    o.Audience = "we-edit-api";
    o.RequireHttpsMetadata = false;
});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("apiPolicy", policy => policy.RequireClaim("api.read"));
//    //options.AddPolicy("staffPolicy", policy => policy.RequireRole("staff", "admin", "cso", "editor", "qc"));
//});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("apiPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("api.read");
    });
});

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder.SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

// Timer for signalr hub
builder.Services.AddSingleton<TimerManager>();
builder.Services.AddSignalR();

var app = builder.Build();

// Migrate and seed database
using (var serviceScope = app.Services.CreateScope())
{
    var productionContextSeed = new ProductionContextSeed(serviceScope.ServiceProvider.GetRequiredService<ProductionContext>(),
        builder.Environment.ContentRootPath,
        "SeedData",
        serviceScope.ServiceProvider.GetService<ILogger<ProductionContextSeed>>());

    //productionContextSeed.Created(); // DB have to created before
    productionContextSeed.Migrate();
    productionContextSeed.SeedAsync().Wait();
}

// swagger in development
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers()
        .RequireAuthorization();
});

app.MapHub<APIHub>("/apishub");

app.Run();
