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
        private readonly IRepository<ProductCategoryProduct> productCategoryProductRepository;
        private readonly IRepository<Warehouse> warehouseRepository;
        private readonly IRepository<WarehouseProduct> warehouseProductRepository;

        public ProductService(IRepository<WarehouseProduct> warehouseProductRepository, IRepository<ProductCategoryProduct> productCategoryProductRepository, IRepository<Warehouse> warehouseRepository, IRepository<ProductCategory> productCategoryRepository, IRepository<Unit> unitRepository, IShopService shopService, IRepository<Product> productRepository, IResponseResultBuilder responseResultBuilder, IRepository<ProductAsset> productAssetRepository)
        {
            this.warehouseProductRepository = warehouseProductRepository;
            this.unitRepository = unitRepository;
            this.shopService = shopService;
            this.productRepository = productRepository;
            this.responseResultBuilder = responseResultBuilder;
            this.productAssetRepository = productAssetRepository;
            this.productCategoryRepository = productCategoryRepository;
            this.warehouseRepository = warehouseRepository;
            this.productCategoryProductRepository = productCategoryProductRepository;
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

        public async Task<bool> RemoveProductAssetByIdAsync(int productId, int assetId)
        {
            var asset = await GetProductAssetByIdAsync(productId, assetId) as ProductAsset;
            if (asset == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Asset");
                return false;
            }
            await productAssetRepository.DeleteAsync(asset);
            return true;
        }

        public async Task<ProductDto?> GetProductOrCurrentUserShopAsync(int productId)
        {
            var product = await getProductOfCurrentUserShop(productId);
            if (product == null) return null;
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
                Categories = product.ProductCategoryProducts != null ? product.ProductCategoryProducts.Select(e => new CategoryDto()
                {
                    Id = e.ProductCategory?.Id ?? 0,
                    Name = e.ProductCategory?.Name ?? ""
                }).ToList() : new List<CategoryDto>(),
                Warehouses = product.WarehouseProducts == null ? null : product.WarehouseProducts.Select(e => new ProductWarehouseDto
                {
                    WarehouseId = e.WarehouseId,
                    Quantity = e.Quantity,
                    WarehouseName = e.Warehouse?.Name

                }).ToList(),
                Assets = assets.Items.Select(e => new SimpleAssetDto { Id = e.Id, AssetType = e.AssetType }).ToList(),
            };
        }

        public async Task<PageData<ProductDto>?> GetProductPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search, SortingModel sortingModel)
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
            }), sortingModel.ToSearchQueries<Product>());

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
        private async Task<Product?> getProductOfCurrentUserShop(int productId)
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
            return product;
        }
        public async Task<bool> SetProductThumbnailAsync(int productId, int assetId)
        {
            var product = await getProductOfCurrentUserShop(productId);
            if (product == null) return false;
            var assets = product.ProductAssets;
            if (assets == null)
            {
                return false;


            }
            var asset = assets.FirstOrDefault(e => e.Id == assetId);
            if (asset == null)
            {
                responseResultBuilder.AddInvalidFieldError("AssetId");
                return false;
            }
            for (var i = 0; i < assets.Count; i++)
            {
                var item = assets.ElementAt(i);
                if (item.Id == asset.Id)
                {
                    item.AssetType = ProductAssetConstants.ThumbnailAssetType;
                }
                else
                {
                    item.AssetType = ProductAssetConstants.SlideAssetType;
                }
            }
            await productRepository.UpdateAsync(product);
            return true;
        }
        public async Task<bool> UpdateProductOfCurrentUserShopAsync(int productId, string? name,
        int? unitId, double? mass, double? pricePerMass,
        double? percentForFamiliarCustomer, double? percentForCustomer, double? priceForFamiliarCustomer,
        double? priceForCustomer, bool? hasAutoCalculatePermission,
        List<int>? categoryIds, List<Tuple<int, double>>? warehouses)
        {
            var product = await getProductOfCurrentUserShop(productId);
            if (product == null) return false;
            product.Name = string.IsNullOrEmpty(name) ? product.Name : name;
            product.UnitId = unitId == null ? product.UnitId : unitId.Value;
            product.Mass = mass == null ? product.Mass : mass.Value;
            product.PricePerMass = pricePerMass == null ? product.PricePerMass : pricePerMass.Value;
            product.PercentForFamiliarCustomer = percentForFamiliarCustomer == null ? product.PercentForFamiliarCustomer : percentForFamiliarCustomer.Value;
            product.PercentForCustomer = percentForCustomer == null ? product.PercentForCustomer : percentForCustomer.Value;
            product.PriceForFamiliarCustomer = priceForFamiliarCustomer == null ? product.PriceForFamiliarCustomer : priceForFamiliarCustomer.Value;
            product.PriceForCustomer = priceForCustomer == null ? product.PriceForCustomer : priceForCustomer.Value;
            product.HasAutoCalculatePermission = hasAutoCalculatePermission == null ? product.HasAutoCalculatePermission : hasAutoCalculatePermission.Value;

            if (categoryIds != null)
            {
                var isAllCategoryIdsValid = true;
                for (var i = 0; i < categoryIds.Count; i++)
                {
                    var categoryId = categoryIds[i];
                    var isExist = await productCategoryRepository.AnyAsync(e => e.ShopId == product.ShopId && e.Id == categoryId);
                    if (!isExist)
                    {
                        responseResultBuilder.AddInvalidFieldError($"CategoryIds[{i}]");
                        isAllCategoryIdsValid = false;
                        break;
                    }
                }
                if (isAllCategoryIdsValid)
                {
                    await productCategoryProductRepository.DeleteRangeByQueryAsync(e => e.ProductId == product.Id);
                    foreach (var categoryId in categoryIds)
                    {
                        await productCategoryProductRepository.CreateIfNotExistsAsync(new ProductCategoryProduct
                        {
                            ProductId = product.Id,
                            ProductCategoryId = categoryId
                        }, e => new
                        {
                            e.ProductCategoryId,
                            e.ProductId,
                        });
                    }
                }

            }
            if (warehouses != null)
            {
                var isAllWarehouseIdsValid = true;
                for (var i = 0; i < warehouses.Count; i++)
                {
                    var warehouseId = warehouses[i].Item1;
                    var isExist = await warehouseRepository.AnyAsync(e => e.ShopId == product.ShopId && e.Id == warehouseId);
                    if (!isExist)
                    {
                        responseResultBuilder.AddInvalidFieldError($"Warehouses[{i}].WarehouseId");
                        isAllWarehouseIdsValid = false;
                        break;
                    }
                }
                if (isAllWarehouseIdsValid)
                {
                    foreach (var tuple in warehouses)
                    {
                        await warehouseProductRepository.CreateOrUpdateAsync(new WarehouseProduct
                        {
                            ProductId = product.Id,
                            WarehouseId = tuple.Item1,
                            Quantity = tuple.Item2,
                        }, e => new
                        {
                            e.ProductId,
                            e.WarehouseId,
                        }, e => new
                        {
                            e.Quantity
                        });
                    }
                }

            }
            await productRepository.UpdateAsync(product);
            return true;
        }
    }
}
