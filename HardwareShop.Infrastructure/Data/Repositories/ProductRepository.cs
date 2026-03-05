using HardwareShop.Application.CQRS.ProductArea.Interfaces;
using HardwareShop.Application.Models;
using HardwareShop.Domain.Models;
using HardwareShop.Infrastructure.Extensions;
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

    public async Task<PageData<Product>> GetPageDataByShopIdAsync(Guid shopId, PagingModel pagingModel, string? search, CancellationToken cancellationToken = default)
    {
        var query = context.Products
            .Include(p => p.Unit)
            .Include(p => p.WarehouseProducts)
            .Where(p => p.ShopId == shopId)
            .Search(string.IsNullOrWhiteSpace(search)
                ? null
                : new SearchQuery<Product>(search, p => new { p.Name }));

        return await query.GetPageDataAsync(pagingModel);
    }


}
