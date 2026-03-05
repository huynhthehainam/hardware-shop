namespace HardwareShop.Application.CQRS.OrderArea.Interfaces
{
    public interface IOrderRepository : IRepository<Domain.Models.Order>
    {
        Task<Domain.Models.Order?> GetDetailedByIdAndShopIdAsync(Guid id, Guid shopId, CancellationToken cancellationToken = default);
    }
}
