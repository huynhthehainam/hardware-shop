
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.CQRS.ProductArea.Interfaces
{
    public interface IProductRepository : IRepository<Domain.Models.Product>
    {
        Task<List<Product>> GetProductsByProductIdsAsync(List<Guid> productIds, Guid shopId, CancellationToken cancellationToken = default);
    }
}