using HardwareShop.Application.CQRS.CustomerArea.Interfaces;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Infrastructure.Data.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(MainDatabaseContext context) : base(context)
    { }

    public async Task<Customer?> GetByIdAndShopIdAsync(Guid id, Guid shopId, CancellationToken cancellationToken = default)
    {
        return await context.Customers
              .Where(c => c.Id == id && c.ShopId == shopId)
              .FirstOrDefaultAsync(cancellationToken);
    }
}