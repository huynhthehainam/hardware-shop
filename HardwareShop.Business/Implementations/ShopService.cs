using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
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
        private readonly IRepository<Account> accountRepository;
        private readonly IRepository<Warehouse> warehouseRepository;
        private readonly ICurrentAccountService currentAccountService;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IHashingPasswordService hashingPasswordService;
        private readonly IRepository<AccountShop> accountShopRepository;
        public ShopService(IRepository<Shop> shopRepository, IRepository<Warehouse> warehouseRepository, ICurrentAccountService currentAccountService, IResponseResultBuilder responseResultBuilder, IRepository<Account> accountRepository, IHashingPasswordService hashingPasswordService, IRepository<AccountShop> accountShopRepository)
        {
            this.shopRepository = shopRepository;
            this.currentAccountService = currentAccountService;
            this.warehouseRepository = warehouseRepository;
            this.responseResultBuilder = responseResultBuilder;
            this.accountRepository = accountRepository;
            this.hashingPasswordService = hashingPasswordService;
            this.accountShopRepository = accountShopRepository;
        }

        public async Task<CreatedAccountDto?> CreateAdminAccountAsync(int id, string username, string password, string? email)
        {
            var shop = await shopRepository.GetItemByQueryAsync(e => e.Id == id);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var account = await accountRepository.CreateIfNotExists(new Account
            {
                Username = username,
                HashedPassword = hashingPasswordService.Hash(password),
                Email = email,
            }, e => new
            {
                e.Username
            });
            if (account == null)
            {
                responseResultBuilder.AddExistedEntityError("Account");
                return null;
            }

            AccountShop accountShop = await accountShopRepository.CreateAsync(new AccountShop
            {

                AccountId = account.Id,
                ShopId = shop.Id,
                Role = ShopAccountRole.Admin
            });

            return new CreatedAccountDto { Id = account.Id };

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

        public async Task<CreatedWarehouseDto?> CreateWarehouseOfCurrentAccountShopAsync(string name, string? address)
        {
            var shop = await GetShopByCurrentAccountIdAsync(ShopAccountRole.Admin);
            if (shop == null)
            {
                return null;
            }
            Warehouse warehouse = await warehouseRepository.CreateAsync(new Warehouse
            {
                Name = name,
                Address = address,
                ShopId = shop.Id
            });


            return new CreatedWarehouseDto { Id = warehouse.Id };

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



        public async Task<ShopDto?> GetShopByAccountIdAsync(int accountId, ShopAccountRole role)
        {
            var acceptedRoles = new List<ShopAccountRole>();
            switch (role)
            {
                case ShopAccountRole.Staff:
                    acceptedRoles = new List<ShopAccountRole> { ShopAccountRole.Staff, ShopAccountRole.Admin };
                    break;
                case ShopAccountRole.Admin:
                    acceptedRoles = new List<ShopAccountRole> { ShopAccountRole.Admin };
                    break;
                default:
                    break;
            }
            var shop = await shopRepository.GetItemByQueryAsync(e =>
         e.ShopAccounts != null ? e.ShopAccounts.Any(s => s.AccountId == accountId && acceptedRoles.Contains(s.Role)) : false);

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

        public Task<ShopDto?> GetShopByCurrentAccountIdAsync(ShopAccountRole role)
        {
            return GetShopByAccountIdAsync(currentAccountService.GetAccountId(), role);
        }
    }
}
