
namespace HardwareShop.Application.CQRS.ShopArea.Interfaces
{
    public interface IShopRepository
    {
        Task<Domain.Models.Shop> AddAsync(Domain.Models.Shop entity, CancellationToken cancellationToken = default);
    }
}