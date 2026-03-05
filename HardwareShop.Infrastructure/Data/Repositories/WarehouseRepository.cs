using HardwareShop.Application.CQRS.WarehouseArea.Interfaces;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Infrastructure.Data.Repositories;

public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(MainDatabaseContext context) : base(context)
    {


    }

    public Task<List<WarehouseProduct>> GetWarehouseProductsByProductUnitIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default)
    {
        return context.WarehouseProducts
              .Where(wp => productIds.Contains(wp.ProductId))
              .ToListAsync(cancellationToken);
    }
}