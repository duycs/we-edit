using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences
{
    public abstract class DbContextBase<T> : DbContext, IDatabaseService where T : class
    {
        public DbContextBase()
        {
        }

        DbSet<T> IDatabaseService.GetDbSet<T>()
        {
            return Set<T>();
        }

        Task IDatabaseService.SaveChanges()
        {
            return Task.FromResult(base.SaveChanges());
        }
    }
}

