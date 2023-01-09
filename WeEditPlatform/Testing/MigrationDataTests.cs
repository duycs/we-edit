using Infrastructure.Persistences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;

namespace Testing
{
    public class MigrationDataTests : TestBase
    {
        private IServiceScope _serviceScope;
        public ProductionContextSeed ProductionContextSeed { get; set; }

        [SetUp]
        public void Setup()
        {
            _serviceScope = _serviceProvider.CreateScope();
            var productionContext = _serviceScope.ServiceProvider.GetRequiredService<ProductionContext>();

            var factory = _serviceScope.ServiceProvider.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger<ProductionContextSeed>();
            ProductionContextSeed = new ProductionContextSeed(productionContext, Environment.CurrentDirectory, "SeedData", logger);

        }

        [Test]
        public void Create()
        {
            try
            {
                ProductionContextSeed.Created();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Test]
        public void Migrate()
        {
            try
            {
                // only use migration for relation database
                var useInMemoryDb = _configuration.GetValue<bool>("UseInMemoryDb");
                if (!useInMemoryDb)
                {
                    ProductionContextSeed.Created();
                    ProductionContextSeed.Migrate();
                }
            }
            catch (Exception ex) { throw; }
        }

        [Test]
        public void Seed()
        {
            try
            {
                ProductionContextSeed.SeedAsync().Wait();
            }
            catch (Exception ex) { throw; }
        }
    }
}
