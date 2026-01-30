
using HardwareShop.Application;
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Infrastructure.Data.Repositories
{
    public class BaseRepository<T>(MainDatabaseContext context) : IRepository<T> where T : EntityBase
    {


        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            context.Set<T>().Add(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}