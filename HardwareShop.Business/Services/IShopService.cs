using HardwareShop.Business.Dtos;
using HardwareShop.Core.Models;
using HardwareShop.Dal.Models;
using Microsoft.AspNetCore.Http;
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
        Task<ShopDto?> GetShopByUserIdAsync(int userId, UserShopRole role = UserShopRole.Staff);
        Task<ShopDto?> GetShopByCurrentUserIdAsync(UserShopRole role = UserShopRole.Staff);
        Task<CreatedShopDto?> CreateShopAsync(string name, string? address);
        Task<ShopAssetDto?> UpdateLogoAsync(int shopId, IFormFile file);
        Task<bool> DeleteShopSoftlyAsync(int shopId);
        Task<CreatedUserDto?> CreateAdminUserAsync(int id, string v1, string v2, string? email);

    }
}
