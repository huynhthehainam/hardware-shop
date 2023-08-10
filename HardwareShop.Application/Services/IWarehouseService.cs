using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Core.Models;

namespace HardwareShop.Application.Services
{
    public interface IWarehouseService
    {
        Task<ApplicationResponse<CreatedWarehouseDto>> CreateWarehouseOfCurrentUserShopAsync(string name, string? address);
        Task<ApplicationResponse<PageData<WarehouseDto>>> GetWarehousesOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<ApplicationResponse> DeleteWarehouseOfCurrentUserShopAsync(int warehouseId);
        Task<ApplicationResponse<WarehouseProductDto>> CreateOrUpdateWarehouseProductAsync(int warehouseId, int productId, double quantity);
    }
}
