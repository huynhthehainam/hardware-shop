
using HardwareShop.Application.CQRS.ShopArea.Interfaces;
using HardwareShop.Domain.Events;
using HardwareShop.Domain.Models;

namespace HardwareShop.Infrastructure.Data.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly MainDatabaseContext context;
        public ShopRepository(MainDatabaseContext context)
        {
            this.context = context;
        }

        public async Task<Shop> AddAsync(Shop entity, CancellationToken cancellationToken = default)
        {
            context.Shops.Add(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}