
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Application
{
    public interface IRepository<T> where T : EntityBase
    {
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    }
}