using HardwareShop.Business.Dtos;
using HardwareShop.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Services
{
    public interface IShopService
    {
        Task<ShopDto?> GetShopByAccountIdAsync(int accountId, ShopAccountRole role = ShopAccountRole.Staff);
        Task<ShopDto?> GetShopByCurrentAccountIdAsync(ShopAccountRole role = ShopAccountRole.Staff);
        Task<CreatedShopDto?> CreateShopAsync(string name, string? address);
        Task<CreatedWarehouseDto?> CreateWarehouseOfCurrentAccountShopAsync(string name, string? address);
        Task<bool> DeleteShopSoftlyAsync(int shopId);
        Task<CreatedAccountDto?> CreateAdminAccountAsync(int id, string v1, string v2, string? email);
    }
}
