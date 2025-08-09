using HardwareShop.Application.Dtos;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using HardwareShop.Application.Models;
using HardwareShop.Infrastructure.Extensions;

namespace HardwareShop.Infrastructure.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IShopService shopService;
        private readonly DbContext db;
        public WarehouseService(IShopService shopService, DbContext db)
        {
            this.shopService = shopService;
            this.db = db;
        }
        public async Task<ApplicationResponse<PageData<WarehouseDto>>> GetWarehousesOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }


            var warehousePageData = db.Set<Warehouse>().Where(e => e.ShopId == shop.Id).Search(search == null ? null : new SearchQuery<Warehouse>(search, e => new
            {
                e.Name,
                e.Address
            })).GetPageData(pagingModel);
            return new(warehousePageData.ConvertToOtherPageData(e => new WarehouseDto(e.Id, e.Name, e.Address)));
        }

        public async Task<ApplicationResponse> DeleteWarehouseOfCurrentUserShopAsync(int warehouseId)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            var warehouses = db.Set<Warehouse>().Where(e => e.ShopId == shop.Id && e.Id == warehouseId).ToArray();
            db.RemoveRange(warehouses);
            db.SaveChanges();
            return new();
        }
        public async Task<ApplicationResponse<CreatedWarehouseDto>> CreateWarehouseOfCurrentUserShopAsync(string name, string? address)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            Warehouse warehouse = new()
            {
                Name = name,
                Address = address,
                ShopId = shop.Id
            };
            db.Add(warehouse);
            db.SaveChanges();

            return new(new CreatedWarehouseDto
            {
                Id = warehouse.Id
            });

        }

        public async Task<ApplicationResponse<WarehouseProductDto>> CreateOrUpdateWarehouseProductAsync(int warehouseId, int productId, double quantity)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            Warehouse? warehouse = await db.Set<Warehouse>().FirstOrDefaultAsync(e => e.ShopId == shop.Id && e.Id == warehouseId);
            if (warehouse == null)
            {
                return new(ApplicationError.CreateNotFoundError("Warehouse"));

            }

            Product? product = await db.Set<Product>().FirstOrDefaultAsync(e => (e.ShopId == shop.Id) && e.Id == productId);
            if (product == null)
            {
                return new(ApplicationError.CreateInvalidError("ProductId"));

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
            return new(new WarehouseProductDto(item.WarehouseId, item.ProductId, item.Quantity));
        }
    }
}
