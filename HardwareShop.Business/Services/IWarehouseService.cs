using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Services
{
    public interface IWarehouseService
    {
        Task<CreatedWarehouseDto?> CreateWarehouseOfCurrentUserShopAsync(string name, string? address);
        Task<PageData<WarehouseDto>?> GetWarehousesOfCurrentUserShopAsync(PagingModel pagingModel, string? search);
        Task<bool> DeleteWarehouseOfCurrentUserShopAsync(int warehouseId);
        Task<WarehouseProductDto?> CreateOrUpdateWarehouseProductAsync(int warehouseId, int productId, double quantity);
    }
}
