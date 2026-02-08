
using HardwareShop.Application;
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Infrastructure.Data.Repositories
{
    public class BaseRepository<T>(MainDatabaseContext context) : IRepository<T> where T : EntityBase
    {
        protected readonly MainDatabaseContext context = context;
        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            context.Set<T>().Add(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}