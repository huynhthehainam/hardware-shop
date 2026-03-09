using HardwareShop.Application.Models;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.CQRS.CustomerArea.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByIdAndShopIdAsync(Guid id, Guid shopId, CancellationToken cancellationToken = default);
        Task<PageData<Customer>> GetPageDataByShopIdAsync(Guid shopId, PagingModel pagingModel, string? search, CancellationToken cancellationToken = default);
    }
}
