using HardwareShop.Application.CQRS.OrderArea.Interfaces;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Infrastructure.Data.Repositories;
public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(MainDatabaseContext context) : base(context)
    {
    }

    public async Task<Order?> GetDetailedByIdAndShopIdAsync(Guid id, Guid shopId, CancellationToken cancellationToken = default)
    {
        return await context.Orders
            .Include(o => o.Shop)
            .Include(o => o.Customer)
                .ThenInclude(c => c!.PhoneCountry)
            .Include(o => o.Details!)
                .ThenInclude(d => d.Product)
            .Include(o => o.Details!)
                .ThenInclude(d => d.Unit)
            .FirstOrDefaultAsync(o => o.Id == id && o.ShopId == shopId, cancellationToken);
    }
}
