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
            ShopDto? shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            Unit? unit = await unitRepository.GetItemByQueryAsync(e => e.Id == unitId);
            if (unit == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Unit");
                return null;
            }
            List<int> validatedCategoryIds = new List<int>();
            if (categoryIds != null)
            {
                for (int i = 0; i < categoryIds.Count; i++)
                {
                    int categoryId = categoryIds[i];
                    bool isExist = await productCategoryRepository.AnyAsync(e => e.Id == categoryId && e.ShopId == shop.Id);
                    if (!isExist)
                    {
                        responseResultBuilder.AddNotFoundEntityError($"CategoryIds[{i}]");
                        return null;
                    }
                    validatedCategoryIds.Add(categoryId);
                }
            }
            List<Tuple<int, double>>? validatedWarehouses = new();
            if (warehouses != null)
            {
                for (int i = 0; i < warehouses.Count; i++)
                {

                    Tuple<int, double> warehouse = warehouses[i];
                    bool isExist = await warehouseRepository.AnyAsync(e => e.Id == warehouse.Item1 && e.ShopId == shop.Id);
                    if (!isExist)
                    {
                        responseResultBuilder.AddNotFoundEntityError($"Warehouses[{i}].Id");
                        return null;
                    }
                    validatedWarehouses.Add(new Tuple<int, double>(warehouse.Item1, warehouse.Item2));
                }
            }
            CreateIfNotExistResponse<Product> createIfNotExistResponse = await productRepository.CreateIfNotExistsAsync(new Product
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

            if (createIfNotExistResponse.IsExist)
            {
                responseResultBuilder.AddInvalidFieldError("Name");
                return null;
            }
            return new CreatedProductDto { Id = createIfNotExistResponse.Entity.Id };
        }

        public async Task<IAssetTable?> GetProductAssetByIdAsync(int productId, int assetId)
        {
            ShopDto? shop = await shopService.GetShopDtoByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            Product? product = await productRepository.GetItemByQueryAsync(e => e.ShopId == shop.Id && e.Id == productId);
            if (product == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Product");
                return null;
            }

            ProductAsset? asset = await productAssetRepository.GetItemByQueryAsync(e => e.Id == assetId && e.ProductId == product.Id);
            return asset;
        }

        public async Task<bool> RemoveProductAssetByIdAsync(int productId, int assetId)
        {
            if (await GetProductAssetByIdAsync(productId, assetId) is not ProductAsset asset)
            {
                responseResultBuilder.AddNotFoundEntityError("Asset");
                return false;
            }
            _ = await productAssetRepository.DeleteAsync(asset);
            return true;
        }

        public async Task<ProductDto?> GetProductDtoOfCurrentUserShopAsync(int productId)
        {
            Product? product = await GetProductOfCurrentUserShop(productId);
            if (product == null)
            {
                return null;
            }

            PageData<SimpleAssetDto> assets = await productAssetRepository.GetDtoPageDataByQueryAsync(new PagingModel(), e => e.ProductId == product.Id, e => new SimpleAssetDto { Id = e.Id, AssetType = e.AssetType });
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
                ProductCategoryIds = product.ProductCategoryProducts != null ? product.ProductCategoryProducts.Select(e => e.ProductCategoryId).ToArray() : Array.Empty<int>(),
                ProductCategoryNames = product.ProductCategoryProducts != null ? product.ProductCategoryProducts.Select(e => e.ProductCategory?.Name).ToArray() : Array.Empty<string>(),
                UnitId = product.UnitId,
                UnitName = product.Unit?.Name,
                OriginalPrice = product.OriginalPrice,
                InventoryNumber = product.InventoryNumber,
                Categories = product.ProductCategoryProducts != null ? product.ProductCategoryProducts.Select(e => new CategoryDto()
                {
                    Id = e.ProductCategory?.Id ?? 0,
                    Name = e.ProductCategory?.Name ?? ""
                }).ToList() : new List<CategoryDto>(),
                Warehouses = product.WarehouseProducts?.Select(e => new ProductWarehouseDto
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
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            PageData<Product> productPageData = await productRepository.GetPageDataByQueryAsync(pagingModel, e => e.ShopId == shop.Id, string.IsNullOrEmpty(search) ? null : new SearchQuery<Product>(search, e => new
            {
                e.Name
            }), sortingModel.ToOrderQueries<Product>());

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
                ProductCategoryIds = e.ProductCategoryProducts != null ? e.ProductCategoryProducts.OrderBy(e => e.ProductCategory?.Name ?? "").Select(e => e.ProductCategoryId).ToArray() : Array.Empty<int>(),
                ProductCategoryNames = e.ProductCategoryProducts != null ? e.ProductCategoryProducts.OrderBy(e => e.ProductCategory?.Name ?? "").Select(e => e.ProductCategory?.Name).ToArray() : Array.Empty<string>(),
                UnitId = e.UnitId,
                UnitName = e.Unit?.Name,
                InventoryNumber = e.InventoryNumber,

            });

        }

        public async Task<IAssetTable?> GetProductThumbnailAsync(int productId)
        {

            ProductAsset? productAsset = await productAssetRepository.GetItemByQueryAsync(e => e.ProductId == productId && (e.Product != null) && e.AssetType == ProductAssetConstants.ThumbnailAssetType);
            if (productAsset == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Asset");
                return null;
            }
            return productAsset;
        }

        public async Task<int?> UploadProductImageOfCurrentUserShopAsync(int productId, string assetType, IFormFile file)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            Product? product = await productRepository.GetItemByQueryAsync(e => e.Id == productId && e.ShopId == shop.Id);
            if (product == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Product");
                return null;
            }

            ProductAsset productAsset = new ProductAsset()
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
        private async Task<Product?> GetProductOfCurrentUserShop(int productId)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync(UserShopRole.Admin);
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return null;
            }
            Product? product = await productRepository.GetItemByQueryAsync(e => e.Id == productId && e.ShopId == shop.Id);
            if (product == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Product");
                return null;
            }
            return product;
        }
        public async Task<bool> SetProductThumbnailAsync(int productId, int assetId)
        {
            Product? product = await GetProductOfCurrentUserShop(productId);
            if (product == null)
            {
                return false;
            }

            ICollection<ProductAsset>? assets = product.ProductAssets;
            if (assets == null)
            {
                return false;


            }
            ProductAsset? asset = assets.FirstOrDefault(e => e.Id == assetId);
            if (asset == null)
            {
                responseResultBuilder.AddInvalidFieldError("AssetId");
                return false;
            }
            for (int i = 0; i < assets.Count; i++)
            {
                ProductAsset item = assets.ElementAt(i);
                item.AssetType = item.Id == asset.Id ? ProductAssetConstants.ThumbnailAssetType : ProductAssetConstants.SlideAssetType;
            }
            _ = await productRepository.UpdateAsync(product);
            return true;
        }
        public async Task<bool> UpdateProductOfCurrentUserShopAsync(int productId, string? name,
        int? unitId, double? mass, double? pricePerMass,
        double? percentForFamiliarCustomer, double? percentForCustomer, double? priceForFamiliarCustomer,
        double? priceForCustomer, bool? hasAutoCalculatePermission,
        List<int>? categoryIds, List<Tuple<int, double>>? warehouses)
        {
            Product? product = await GetProductOfCurrentUserShop(productId);
            if (product == null)
            {
                return false;
            }

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
                bool isAllCategoryIdsValid = true;
                for (int i = 0; i < categoryIds.Count; i++)
                {
                    int categoryId = categoryIds[i];
                    bool isExist = await productCategoryRepository.AnyAsync(e => e.ShopId == product.ShopId && e.Id == categoryId);
                    if (!isExist)
                    {
                        responseResultBuilder.AddInvalidFieldError($"CategoryIds[{i}]");
                        isAllCategoryIdsValid = false;
                        break;
                    }
                }
                if (isAllCategoryIdsValid)
                {
                    _ = await productCategoryProductRepository.DeleteRangeByQueryAsync(e => e.ProductId == product.Id);
                    foreach (int categoryId in categoryIds)
                    {
                        _ = await productCategoryProductRepository.CreateIfNotExistsAsync(new ProductCategoryProduct
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
                bool isAllWarehouseIdsValid = true;
                for (int i = 0; i < warehouses.Count; i++)
                {
                    int warehouseId = warehouses[i].Item1;
                    bool isExist = await warehouseRepository.AnyAsync(e => e.ShopId == product.ShopId && e.Id == warehouseId);
                    if (!isExist)
                    {
                        responseResultBuilder.AddInvalidFieldError($"Warehouses[{i}].WarehouseId");
                        isAllWarehouseIdsValid = false;
                        break;
                    }
                }
                if (isAllWarehouseIdsValid)
                {
                    foreach (Tuple<int, double> tuple in warehouses)
                    {
                        _ = await warehouseProductRepository.CreateOrUpdateAsync(new WarehouseProduct
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
            _ = await productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> AddPricePerMassOfCurrentUserShopAsync(List<int> categoryIds, double amountOfCash)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return false;
            }
            Unit? cashUnit = shop.CashUnit;
            if (cashUnit == null)
            {
                responseResultBuilder.AddNotFoundEntityError("Shop");
                return false;
            }
            PageData<Product> productPageData = await productRepository.GetPageDataByQueryAsync(new PagingModel(), e => e.ShopId == shop.Id && e.HasAutoCalculatePermission && e.ProductCategoryProducts != null && e.ProductCategoryProducts.Any(c => categoryIds.Contains(c.ProductCategoryId)));
            List<Product> products = productPageData.Items;
            foreach (Product product in products)
            {
                if (product.HasAutoCalculatePermission)
                {
                    if (product.Mass != null && product.PricePerMass != null)
                    {
                        product.PricePerMass += amountOfCash;
                        if (product.PercentForFamiliarCustomer != null)
                        {
                            double price = product.Mass.Value * product.PricePerMass.Value * (100 + product.PercentForFamiliarCustomer.Value) / 100;
                            price = cashUnit.RoundValue(price);
                            product.PriceForFamiliarCustomer = price;
                        }
                        if (product.PercentForCustomer != null)
                        {
                            double price = product.Mass.Value * product.PricePerMass.Value * (100 + product.PercentForCustomer.Value) / 100;
                            price = cashUnit.RoundValue(price);
                            product.PriceForCustomer = price;
                        }
                    }
                }
                else
                {
                    if (product.PricePerMass != null)
                    {
                        product.PricePerMass += amountOfCash;
                    }
                }
                _ = await productRepository.UpdateAsync(product);
            }
            return true;
        }

        public async Task<bool> SoftlyDeleteProductOfCurrentUserShopAsync(int id)
        {
            Product? product = await GetProductOfCurrentUserShop(id);
            return product == null ? false : await productRepository.DeleteSoftlyAsync(product);
        }
    }
}
