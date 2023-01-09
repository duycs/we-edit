using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Data.SqlClient;

namespace Infrastructure.Persistences
{
    public abstract class SeedDataBase
    {
        private DbContext _dbContext;
        private string _contentRootPath;
        private string _seedDataFolder;

        protected SeedDataBase(DbContext dbContext, string contentRootPath, string seedDataFolder)
        {
            _dbContext = dbContext;
            _contentRootPath = contentRootPath;
            _seedDataFolder = seedDataFolder;
        }

        /// <summary>
        /// Same as run CLI: dotnet ef database update
        /// </summary>
        public void Migrate()
        {
            _dbContext.Database.Migrate();
        }

        /// <summary>
        /// Same as run CLI: dotnet ef migrations add [AddedFileName] -o DataAccess/Migrations
        /// </summary>
        public void Created()
        {
            _dbContext.Database.EnsureCreated();
        }

        public string GetPathToFile(string csvFile)
        {
            return Path.Combine(_contentRootPath, _seedDataFolder, csvFile);
        }

        /// <summary>
        /// Create AsyncRetryPolicy for retry handler
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="prefix"></param>
        /// <param name="retries"></param>
        /// <returns></returns>
        public AsyncRetryPolicy CreatePolicy<T>(ILogger<T> _logger, string prefix, int retries = 3) where T : class
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        _logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }

        /// <summary>
        /// Should be override implement
        /// </summary>
        /// <returns></returns>
        public abstract Task SeedAsync();
    }
}
