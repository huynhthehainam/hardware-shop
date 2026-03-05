using HardwareShop.Application.CQRS.CustomerArea.Interfaces;
using HardwareShop.Application.Models;
using HardwareShop.Domain.Models;
using HardwareShop.Infrastructure.Extensions;
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

    public async Task<PageData<Customer>> GetPageDataByShopIdAsync(Guid shopId, PagingModel pagingModel, string? search, CancellationToken cancellationToken = default)
    {
        var query = context.Customers
            .Include(c => c.PhoneCountry)
            .Where(c => c.ShopId == shopId)
            .Search(string.IsNullOrWhiteSpace(search)
                ? null
                : new SearchQuery<Customer>(search, c => new { c.Name, c.Phone, c.Address }));

        return await query.GetPageDataAsync(pagingModel);
    }
}
