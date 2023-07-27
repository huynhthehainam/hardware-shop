using HardwareShop.Application.Dtos;
using HardwareShop.Core.Models;

namespace HardwareShop.Application.Services
{
    public interface IWarehouseService
    {
        Task<CreatedWarehouseDto?> CreateWarehouseOfCurrentUserShopAsync(string name, string? address);
        Task<PageData<WarehouseDto>?> GetWarehousesOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<bool> DeleteWarehouseOfCurrentUserShopAsync(int warehouseId);
        Task<WarehouseProductDto?> CreateOrUpdateWarehouseProductAsync(int warehouseId, int productId, double quantity);
    }
}
