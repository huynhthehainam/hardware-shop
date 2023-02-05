using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Implementations;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Implementations
{
    public sealed class ShopService : IShopService
    {
        private readonly IRepository<Shop> shopRepository;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Warehouse> warehouseRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IHashingPasswordService hashingPasswordService;
        private readonly IRepository<UserShop> userShopRepository;

        public ShopService(IRepository<Shop> shopRepository, IRepository<Warehouse> warehouseRepository, ICurrentUserService currentUserService, IResponseResultBuilder responseResultBuilder, IRepository<User> userRepository, IHashingPasswordService hashingPasswordService, IRepository<UserShop> userShopRepository)
        {
            this.shopRepository = shopRepository;
            this.currentUserService = currentUserService;
            this.warehouseRepository = warehouseRepository;
            this.responseResultBuilder = responseResultBuilder;
            this.userRepository = userRepository;
            this.hashingPasswordService = hashingPasswordService;
            this.userShopRepository = userShopRepository;
        }

        public async Task<CreatedUserDto?> CreateAdminUserAsync(int id, string username, string password, string? email)
        {
            var shop = await shopRepository.GetItemByQueryAsync(e => e.Id == id);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var user = await userRepository.CreateIfNotExists(new User
            {
                Username = username,
                HashedPassword = hashingPasswordService.Hash(password),
                Email = email,
            }, e => new
            {
                e.Username
            });
            if (user == null)
            {
                responseResultBuilder.AddExistedEntityError("User");
                return null;
            }

            UserShop userShop = await userShopRepository.CreateAsync(new UserShop
            {

                UserId = user.Id,
                ShopId = shop.Id,
                Role = UserShopRole.Admin
            });

            return new CreatedUserDto { Id = user.Id };

        }

        public async Task<CreatedShopDto?> CreateShopAsync(string name, string? address)
        {
            var shop = await shopRepository.CreateIfNotExists(new Shop
            {
                Name = name,
                Address = address
            },
                e => new
                {
                    e.Name,
                });
            if (shop == null)
            {
                responseResultBuilder.AddExistedEntityError("Shop");
                return null;
            }
            return new CreatedShopDto { Id = shop.Id };
        }



        public async Task<bool> DeleteShopSoftlyAsync(int shopId)
        {
            var shop = await shopRepository.GetItemByQueryAsync(e => e.Id == shopId);
            if (shop == null)
            {
                this.responseResultBuilder.AddNotFoundEntityError("Shop");
                return false;
            }
            return await shopRepository.DeleteSoftlyAsync(shop);
        }



        public async Task<ShopDto?> GetShopByUserIdAsync(int userId, UserShopRole role)
        {
            var acceptedRoles = new List<UserShopRole>();
            switch (role)
            {
                case UserShopRole.Staff:
                    acceptedRoles = new List<UserShopRole> { UserShopRole.Staff, UserShopRole.Admin };
                    break;
                case UserShopRole.Admin:
                    acceptedRoles = new List<UserShopRole> { UserShopRole.Admin };
                    break;
                default:
                    break;
            }
            var shop = await shopRepository.GetItemByQueryAsync(e =>
         e.UserShops != null ? e.UserShops.Any(s => s.UserId == userId && acceptedRoles.Contains(s.Role)) : false);

            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            return new ShopDto
            {
                Id = shop.Id
            };
        }

        public Task<ShopDto?> GetShopByCurrentUserIdAsync(UserShopRole role)
        {
            return GetShopByUserIdAsync(currentUserService.GetUserId(), role);
        }

        public Task<ShopAssetDto?> UpdateLogoAsync(int shopId, IFormFile file)
        {
            var shop = await shopRepository.GetItemByQueryAsync(e => e.Id == shopId);
            if (shop == null)
            {
                this.responseResultBuilder.AddNotFoundEntityError("Shop");
                return false;
            }
        }
    }
}
