using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Business.Implementations
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IShopService shopService;
        private readonly IRepository<Warehouse> warehouseRepository;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IRepository<WarehouseProduct> warehouseProductRepository;
        private readonly IRepository<Product> productRepository;
        public WarehouseService(IShopService shopService, IRepository<Warehouse> warehouseRepository, IResponseResultBuilder responseResultBuilder, IRepository<WarehouseProduct> warehouseProductRepository, IRepository<Product> productRepository)
        {
            this.shopService = shopService;
            this.warehouseRepository = warehouseRepository;
            this.responseResultBuilder = responseResultBuilder;
            this.warehouseProductRepository = warehouseProductRepository;
            this.productRepository = productRepository;
        }
        public async Task<PageData<WarehouseDto>?> GetWarehousesOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }

            PageData<Warehouse> warehousePageData = await warehouseRepository.GetPageDataByQueryAsync(pagingModel, e => e.ShopId == shop.Id, search == null ? null : new SearchQuery<Warehouse>(search, e => new { e.Name, e.Address }));

            return PageData<WarehouseDto>.ConvertFromOtherPageData(warehousePageData, e => new WarehouseDto(e.Id, e.Name, e.Address));
        }

        public async Task<bool> DeleteWarehouseOfCurrentUserShopAsync(int warehouseId)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return false;
            }
            return await warehouseRepository.DeleteByQueryAsync(e => e.ShopId == shop.Id && e.Id == warehouseId);
        }
        public async Task<CreatedWarehouseDto?> CreateWarehouseOfCurrentUserShopAsync(string name, string? address)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
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

        public async Task<WarehouseProductDto?> CreateOrUpdateWarehouseProductAsync(int warehouseId, int productId, double quantity)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            Warehouse? warehouse = await warehouseRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.Id == warehouseId);
            if (warehouse == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Warehouse");
                return null;
            }

            Product? product = await productRepository.GetItemByQueryAsync(e => (e.ShopId == shop.Id) && e.Id == productId);
            if (product == null)
            {
                responseResultBuilder.AddInvalidFieldError("ProductId");
                return null;
            }
            CreateOrUpdateResponse<WarehouseProduct> createOrUpdateResponse = await warehouseProductRepository.CreateOrUpdateAsync(new WarehouseProduct { ProductId = productId, Quantity = quantity, WarehouseId = warehouseId }, e => new
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
