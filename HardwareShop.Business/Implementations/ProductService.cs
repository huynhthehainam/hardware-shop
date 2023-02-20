using HardwareShop.Business.Dtos;
using HardwareShop.Business.Extensions;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using Microsoft.AspNetCore.Http;

namespace HardwareShop.Business.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IShopService shopService;
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<ProductAsset> productAssetRepository;
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly IRepository<Unit> unitRepository;
        private readonly IRepository<ProductCategory> productCategoryRepository;
        private readonly IRepository<Warehouse> warehouseRepository;
        public ProductService(IRepository<Warehouse> warehouseRepository, IRepository<ProductCategory> productCategoryRepository, IRepository<Unit> unitRepository, IShopService shopService, IRepository<Product> productRepository, IResponseResultBuilder responseResultBuilder, IRepository<ProductAsset> productAssetRepository)
        {
            this.unitRepository = unitRepository;
            this.shopService = shopService;
            this.productRepository = productRepository;
            this.responseResultBuilder = responseResultBuilder;
            this.productAssetRepository = productAssetRepository;
            this.productCategoryRepository = productCategoryRepository;
            this.warehouseRepository = warehouseRepository;
        }

        public async Task<CreatedProductDto?> CreateProductOfShopAsync(string name, int unitId, double? mass, double? pricePerMass, double? percentForFamiliarCustomer, double? percentForCustomer, double? priceForFamiliarCustomer, double priceForCustomer, bool hasAutoCalculatePermission, List<int>? categoryIds,
         List<Tuple<int, double>>? warehouses)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var unit = await unitRepository.GetItemByQueryAsync(e => e.Id == unitId);
            if (unit == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Unit");
                return null;
            }
            var validatedCategoryIds = new List<int>();
            if (categoryIds != null)
            {
                for (var i = 0; i < categoryIds.Count; i++)
                {
                    var categoryId = categoryIds[i];
                    var isExist = await productCategoryRepository.AnyAsync(e => e.Id == categoryId && e.ShopId == shop.Id);
                    if (!isExist)
                    {
                        responseResultBuilder.AddNotFoundEntityError($"CategoryIds[{i}]");
                        return null;
                    }
                    validatedCategoryIds.Add(categoryId);
                }
            }
            List<Tuple<int, double>>? validatedWarehouses = new List<Tuple<int, double>>();
            if (warehouses != null)
            {
                for (var i = 0; i < warehouses.Count; i++)
                {

                    var warehouse = warehouses[i];
                    var isExist = await warehouseRepository.AnyAsync(e => e.Id == warehouse.Item1 && e.ShopId == shop.Id);
                    if (!isExist)
                    {
                        responseResultBuilder.AddNotFoundEntityError($"Warehouses[{i}].Id");
                        return null;
                    }
                    validatedWarehouses.Add(new Tuple<int, double>(warehouse.Item1, warehouse.Item2));
                }
            }
            var product = await productRepository.CreateIfNotExistsAsync(new Product
            {
                Name = name,
                PricePerMass = pricePerMass,
                PercentForFamiliarCustomer = percentForFamiliarCustomer,
                PercentForCustomer = percentForCustomer,
                PriceForFamiliarCustomer = priceForFamiliarCustomer,
                PriceForCustomer = priceForCustomer,
                ShopId = shop.Id,
                UnitId = unit.Id,
                HasAutoCalculatePermission = hasAutoCalculatePermission,
                ProductCategoryProducts = validatedCategoryIds.Select(e => new ProductCategoryProduct
                {
                    ProductCategoryId = e,

                }).ToList(),
                WarehouseProducts = validatedWarehouses.Select(e => new WarehouseProduct { WarehouseId = e.Item1, Quantity = e.Item2 }).ToList(),
            }, e => new
            {
                e.Name
            });

            if (product == null)
            {
                responseResultBuilder.AddInvalidFieldError("Name");
                return null;
            }
            return new CreatedProductDto { Id = product.Id };
        }

        public async Task<IAssetTable?> GetProductAssetByIdAsync(int productId, int assetId)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var product = await productRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.Id == productId);
            if (product == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Product");
                return null;
            }

            var asset = await productAssetRepository.GetItemByQueryAsync(e => e.Id == assetId && e.ProductId == product.Id);
            return asset;
        }

        public async Task<ProductDto?> GetProductOrCurrentUserShopAsync(int productId)
        {
            var shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var product = await productRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.Id == productId);
            if (product == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Product");
                return null;
            }
            var assets = await productAssetRepository.GetDtoPageDataByQueryAsync<SimpleAssetDto>(new PagingModel(), e => e.ProductId == product.Id, e => new SimpleAssetDto { Id = e.Id, AssetType = e.AssetType });
            return new ProductDto
            {
                HasAutoCalculatePermission = product.HasAutoCalculatePermission,
                Name = product.Name,
                Id = product.Id,
                Mass = product.Mass,
                PercentForCustomer = product.PercentForCustomer,
                PercentForFamiliarCustomer = product.PercentForFamiliarCustomer,
                PriceForCustomer = product.PriceForCustomer,
                PriceForFamiliarCustomer = product.PriceForFamiliarCustomer,
                PricePerMass = product.PricePerMass,
                ProductCategoryIds = product.ProductCategoryProducts != null ? product.ProductCategoryProducts.Select(e => e.ProductCategoryId).ToArray() : new int[0],
                ProductCategoryNames = product.ProductCategoryProducts != null ? product.ProductCategoryProducts.Select(e => e.ProductCategory?.Name).ToArray() : new string[0],
                UnitId = product.UnitId,
                UnitName = product.Unit?.Name,
                Warehouses = product.WarehouseProducts == null ? null : product.WarehouseProducts.Select(e => new ProductWarehouseDto
                {
                    WarehouseId = e.WarehouseId,
                    Quantity = e.Quantity,
                    WarehouseName =e.Warehouse?.Name
                    
                }).ToList(),
                Assets = assets.Items.Select(e => new SimpleAssetDto { Id = e.Id, AssetType = e.AssetType }).ToList(),
            };
        }

        public async Task<PageData<ProductDto>?> GetProductPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search)
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            PageData<Product> productPageData = await productRepository.GetPageDataByQueryAsync(pagingModel, e => (e.ShopId == shop.Id), string.IsNullOrEmpty(search) ? null : new SearchQuery<Product>(search, e => new
            {
                e.Name
            }));

            return PageData<ProductDto>.ConvertFromOtherPageData(productPageData, e => new ProductDto
            {
                Name = e.Name,
                Id = e.Id,
                Mass = e.Mass,
                PercentForCustomer = e.PercentForCustomer,
                PercentForFamiliarCustomer = e.PercentForFamiliarCustomer,
                PriceForCustomer = e.PriceForCustomer,
                PriceForFamiliarCustomer = e.PriceForFamiliarCustomer,
                PricePerMass = e.PricePerMass,
                ProductCategoryIds = e.ProductCategoryProducts != null ? e.ProductCategoryProducts.Select(e => e.ProductCategoryId).ToArray() : new int[0],
                ProductCategoryNames = e.ProductCategoryProducts != null ? e.ProductCategoryProducts.Select(e => e.ProductCategory?.Name).ToArray() : new string[0],
                UnitId = e.UnitId,
                UnitName = e.Unit?.Name

            });

        }

        public async Task<IAssetTable?> GetProductThumbnailAsync(int productId)
        {

            var productAsset = await productAssetRepository.GetItemByQueryAsync(e => e.ProductId == productId && (e.Product != null) && e.AssetType == ProductAssetConstants.ThumbnailAssetType);
            if (productAsset == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Asset");
                return null;
            }
            return productAsset;
        }

        public async Task<int?> UploadProductImageOfCurrentUserShopAsync(int productId, string assetType, IFormFile file)
        {
            var shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            var product = await productRepository.GetItemByQueryAsync(e => e.Id == productId && e.ShopId == shop.Id);
            if (product == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Product");
                return null;
            }

            var productAsset = new ProductAsset()
            {
                AssetType = assetType,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                ProductId = product.Id,
            };
            productAsset = file.ConvertToAsset(productAsset);
            productAsset = await productAssetRepository.CreateAsync(productAsset);
            return productAsset.Id;
        }
    }
}
