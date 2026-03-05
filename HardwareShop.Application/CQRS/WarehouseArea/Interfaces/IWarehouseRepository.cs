
using HardwareShop.Domain.Models;

namespace HardwareShop.Application.CQRS.WarehouseArea.Interfaces
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<List<WarehouseProduct>> GetWarehouseProductsByProductUnitIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default);
    }
}