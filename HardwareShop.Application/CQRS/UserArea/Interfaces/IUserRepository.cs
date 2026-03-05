using HardwareShop.Domain.Models;

namespace HardwareShop.Application.CQRS.UserArea.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<Shop?> GetShopByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}