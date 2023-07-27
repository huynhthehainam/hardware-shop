using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Core.Extensions;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Application.Implementations
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IShopService shopService;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly DbContext db;
        public WarehouseService(IShopService shopService, IResponseResultBuilder responseResultBuilder, DbContext db)
        {
            this.shopService = shopService;
            this.responseResultBuilder = responseResultBuilder;
            this.db = db;
        }
        public async Task<PageData<WarehouseDto>?> GetWarehousesOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }


            var warehousePageData = db.Set<Warehouse>().Where(e => e.ShopId == shop.Id).Search(search == null ? null : new SearchQuery<Warehouse>(search, e => new
            {
                e.Name,
                e.Address
            })).GetPageData(pagingModel);
            return warehousePageData.ConvertToOtherPageData(e => new WarehouseDto(e.Id, e.Name, e.Address));
        }

        public async Task<bool> DeleteWarehouseOfCurrentUserShopAsync(int warehouseId)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return false;
            }
            var warehouses = db.Set<Warehouse>().Where(e => e.ShopId == shop.Id && e.Id == warehouseId).ToArray();
            db.RemoveRange(warehouses);
            db.SaveChanges();
            return true;
        }
        public async Task<CreatedWarehouseDto?> CreateWarehouseOfCurrentUserShopAsync(string name, string? address)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                return null;
            }
            Warehouse warehouse = new Warehouse
            {
                Name = name,
                Address = address,
                ShopId = shop.Id
            };
            db.Add(warehouse);
            db.SaveChanges();

            return new CreatedWarehouseDto { Id = warehouse.Id };

        }

        public async Task<WarehouseProductDto?> CreateOrUpdateWarehouseProductAsync(int warehouseId, int productId, double quantity)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            Warehouse? warehouse = await db.Set<Warehouse>().FirstOrDefaultAsync(e => e.ShopId == shop.Id && e.Id == warehouseId);
            if (warehouse == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Warehouse");
                return null;
            }

            Product? product = await db.Set<Product>().FirstOrDefaultAsync(e => (e.ShopId == shop.Id) && e.Id == productId);
            if (product == null)
            {
                responseResultBuilder.AddInvalidFieldError("ProductId");
                return null;
            }
            CreateOrUpdateResponse<WarehouseProduct> createOrUpdateResponse = db.CreateOrUpdate(new WarehouseProduct { ProductId = productId, Quantity = quantity, WarehouseId = warehouseId }, e => new
            {
                e.ProductId,
                e.WarehouseId
            }, e => new
            {
                e.Quantity
            });
            WarehouseProduct item = createOrUpdateResponse.Entity;
            return new WarehouseProductDto(item.WarehouseId, item.ProductId, item.Quantity);
        }
    }
}
