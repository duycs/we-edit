using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences
{
    public interface IDatabaseService
    {
        DbSet<T> GetDbSet<T>() where T : EntityBase;
        Task SaveChanges();
    }
}
