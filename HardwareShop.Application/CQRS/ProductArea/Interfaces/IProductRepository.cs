
using HardwareShop.Application.Models;
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.CQRS.ProductArea.Interfaces
{
    public interface IProductRepository : IRepository<Domain.Models.Product>
    {
        Task<List<Product>> GetProductsByProductIdsAsync(List<Guid> productIds, Guid shopId, CancellationToken cancellationToken = default);
        Task<PageData<Product>> GetPageDataByShopIdAsync(Guid shopId, PagingModel pagingModel, string? search, CancellationToken cancellationToken = default);
    }
}
