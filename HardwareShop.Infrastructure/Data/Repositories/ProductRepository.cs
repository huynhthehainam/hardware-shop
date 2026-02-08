using HardwareShop.Application.CQRS.ProductArea.Interfaces;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Infrastructure.Data.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(MainDatabaseContext context) : base(context)
    {
    }

    public Task<List<Product>> GetProductsByProductIdsAsync(List<Guid> productIds, Guid shopId, CancellationToken cancellationToken = default)
    {
        return context.Products
              .Where(p => productIds.Contains(p.Id) && p.ShopId == shopId)
              .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductUnit>> GetProductUnitsByProductAndUnitIdsAsync(List<(Guid ProductId, int UnitId)> productAndUnitIds, CancellationToken cancellationToken = default)
    {
        return context.ProductUnits.AsEnumerable()
                 .Where(pu => productAndUnitIds.Any(id => id.ProductId == pu.ProductId && id.UnitId == pu.UnitId))
                 .ToList();
    }
}