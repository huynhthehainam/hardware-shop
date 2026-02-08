
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Application
{
    public interface IRepository<T> where T : EntityBase
    {
        Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}