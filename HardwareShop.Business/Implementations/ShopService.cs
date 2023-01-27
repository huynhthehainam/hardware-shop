using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Implementations
{
    public sealed class ShopService : IShopService
    {
        private readonly IRepository<Shop> shopRepository;
        public ShopService(IRepository<Shop> shopRepository)
        {
            this.shopRepository = shopRepository;
        }
        public async Task<CreatedShopDto?> CreateShopAsync(string name, string? address)
        {
            if (await shopRepository.AnyAsync(e => e.Name == name))
            {
                return null;
            }

            var shop = await shopRepository.CreateAsync(new Shop
            {
                Name = name,
                Address = address,
            });

            return new CreatedShopDto { Id = shop.Id };
        }
    }
}
