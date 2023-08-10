using HardwareShop.Application.Dtos;
using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using HardwareShop.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IShopService shopService;
        private readonly DbContext db;
        private readonly IDistributedCache distributedCache;

        public ProductService(IShopService shopService, IDistributedCache distributedCache, DbContext context)
        {
            this.shopService = shopService;
            this.distributedCache = distributedCache;
            this.db = context;
        }

        public async Task<ApplicationResponse<CreatedProductDto>> CreateProductOfShopAsync(string name, int unitId, double? mass, double? pricePerMass, double? percentForFamiliarCustomer, double? percentForCustomer, double? priceForFamiliarCustomer, double priceForCustomer, bool hasAutoCalculatePermission, List<int>? categoryIds,
         List<Tuple<int, double>>? warehouses)
        {
            ShopDto? shop = await shopService.GetShopDtoByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));


            }
            Unit? unit = await db.Set<Unit>().FirstOrDefaultAsync(e => e.Id == unitId);
            if (unit == null)
            {
                return new ApplicationResponse<CreatedProductDto>
                {
                    Error = ApplicationError.CreateNotFoundError("Unit")
                };
            }
            List<int> validatedCategoryIds = new List<int>();
            if (categoryIds != null)
            {
                for (int i = 0; i < categoryIds.Count; i++)
                {
                    int categoryId = categoryIds[i];
                    bool isExist = await db.Set<ProductCategory>().AnyAsync(e => e.Id == categoryId && e.ShopId == shop.Id);
                    if (!isExist)
                    {
                        return new ApplicationResponse<CreatedProductDto>
                        {
                            Error = ApplicationError.CreateInvalidError($"CategoryIds[{i}]")
                        };
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
                    bool isExist = await db.Set<Warehouse>().AnyAsync(e => e.Id == warehouse.Item1 && e.ShopId == shop.Id);
                    if (!isExist)
                    {

                        return new ApplicationResponse<CreatedProductDto>
                        {
                            Error = ApplicationError.CreateInvalidError($"Warehouses[{i}].Id")
                        };

                    }
                    validatedWarehouses.Add(new Tuple<int, double>(warehouse.Item1, warehouse.Item2));
                }
            }
            CreateIfNotExistResponse<Product> createIfNotExistResponse = db.CreateIfNotExists(new Product
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

                return new ApplicationResponse<CreatedProductDto>
                {
                    Error = ApplicationError.CreateInvalidError("Name")
                };
            }
            return new ApplicationResponse<CreatedProductDto>
            {
                Result = new CreatedProductDto { Id = createIfNotExistResponse.Entity.Id }
            };
        }

        public async Task<ApplicationResponse<CachedAssetDto>> GetProductAssetByIdAsync(int productId, int assetId)
        {
            ShopDto? shop = await shopService.GetShopDtoByCurrentUserIdAsync();
            if (shop == null)
            {
                return new()
                {
                    Error = ApplicationError.CreateNotFoundError("Shop")
                };

            }
            Product? product = await db.Set<Product>().FirstOrDefaultAsync(e => e.ShopId == shop.Id && e.Id == productId);
            if (product == null)
            {
                return new()
                {
                    Error = ApplicationError.CreateNotFoundError("Product")
                };

            }

            ProductAsset? asset = await db.Set<ProductAsset>().FirstOrDefaultAsync(e => e.Id == assetId && e.ProductId == product.Id);
            if (asset == null)
            {
                return new()
                {
                    Error = ApplicationError.CreateNotFoundError("Asset")
                };

            }
            return new()
            {
                Result = db.GetCachedAssetById(distributedCache, asset.AssetId)
            };
        }

        public async Task<ApplicationResponse> RemoveProductAssetByIdAsync(int productId, int assetId)
        {
            ShopDto? shop = await shopService.GetShopDtoByCurrentUserIdAsync();
            if (shop == null)
            {
                return new() { Error = ApplicationError.CreateNotFoundError("Shop") };

            }
            Product? product = await db.Set<Product>().FirstOrDefaultAsync(e => e.ShopId == shop.Id && e.Id == productId);
            if (product == null)
            {
                return new() { Error = ApplicationError.CreateNotFoundError("Product") };

            }

            ProductAsset? asset = await db.Set<ProductAsset>().FirstOrDefaultAsync(e => e.Id == assetId && e.ProductId == product.Id);
            if (asset == null)
            {
                return new() { Error = ApplicationError.CreateNotFoundError("Asset") };
            }
            _ = db.Remove(asset);
            db.SaveChanges();
            return new();
        }

        public async Task<ApplicationResponse<ProductDto>> GetProductDtoOfCurrentUserShopAsync(int productId)
        {
            var response = await GetProductOfCurrentUserShop(productId);
            if (response.Result == null) return new ApplicationResponse<ProductDto> { Error = response.Error };
            var product = response.Result;


            return new()
            {
                Result = new ProductDto
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
                    Assets = product.ProductAssets?.Select(e => new SimpleAssetDto { Id = e.Id, AssetType = e.AssetType }).ToList(),

                }
            };
        }

        public async Task<ApplicationResponse<PageData<ProductDto>>> GetProductPageDataOfCurrentUserShopAsync(PagingModel pagingModel, string? search, SortingModel sortingModel)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new() { Error = ApplicationError.CreateNotFoundError("Shop") };

            }
            var productPageData = db.Set<Product>().Include(e => e.ProductCategoryProducts).Include(e => e.ProductAssets).Include(e => e.WarehouseProducts).Include(e => e.Unit)
            .Where(e => e.ShopId == shop.Id).Search(string.IsNullOrEmpty(search) ? null : new SearchQuery<Product>(search, e => new
            {
                e.Name
            })).GetPageData(pagingModel, sortingModel.ToOrderQueries<Product>());

            return new()
            {
                Result = productPageData.ConvertToOtherPageData(e => new ProductDto
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
                    ThumbnailAssetId = e.ProductAssets?.FirstOrDefault(e => e.AssetType == ProductAssetConstants.ThumbnailAssetType)?.AssetId,
                })
            };


        }

        public async Task<ApplicationResponse<CachedAssetDto>> GetProductThumbnailAsync(int productId)
        {

            ProductAsset? asset = await db.Set<ProductAsset>().FirstOrDefaultAsync(e => e.ProductId == productId && (e.Product != null) && e.AssetType == ProductAssetConstants.ThumbnailAssetType);
            if (asset == null)
            {

                return new()
                {
                    Error = ApplicationError.CreateNotFoundError("Asset")
                };
            }
            return new() { Result = db.GetCachedAssetById(distributedCache, asset.AssetId) };
        }

        public async Task<ApplicationResponse<int>> UploadProductImageOfCurrentUserShopAsync(int productId, string assetType, AssetDto file)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new() { Error = ApplicationError.CreateNotFoundError("Shop") };
            }
            Product? product = await db.Set<Product>().FirstOrDefaultAsync(e => e.Id == productId && e.ShopId == shop.Id);
            if (product == null)
            {
                return new() { Error = ApplicationError.CreateNotFoundError("Product") };

            }

            ProductAsset productAsset = new()
            {
                AssetType = assetType,
                ProductId = product.Id,
            };
            productAsset = file.ConvertToAsset(productAsset);
            var createOrUpdateAssetResponse = db.CreateOrUpdateAsset(productAsset, e => new { }, e => new { });
            return new() { Result = createOrUpdateAssetResponse.Entity.Id };
        }
        private async Task<ApplicationResponse<Product>> GetProductOfCurrentUserShop(int productId)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));


            }
            Product? product = await db.Set<Product>().FirstOrDefaultAsync(e => e.Id == productId && e.ShopId == shop.Id);
            if (product == null)
            {
                return new(ApplicationError.CreateNotFoundError("Product"));

            }
            return new(product);
        }
        public async Task<ApplicationResponse> SetProductThumbnailAsync(int productId, int assetId)
        {
            var response = await GetProductOfCurrentUserShop(productId);
            if (response.Result == null) return new() { Error = response.Error };
            var product = response.Result;

            ICollection<ProductAsset>? assets = product.ProductAssets;
            if (assets == null)
            {
                return new ApplicationResponse(ApplicationError.CreateNotFoundError("Assets"));
            }
            ProductAsset? asset = assets.FirstOrDefault(e => e.Id == assetId);
            if (asset == null)
            {
                return new(ApplicationError.CreateInvalidError("AssetId"));

            }
            for (int i = 0; i < assets.Count; i++)
            {
                ProductAsset item = assets.ElementAt(i);
                item.AssetType = item.Id == asset.Id ? ProductAssetConstants.ThumbnailAssetType : ProductAssetConstants.SlideAssetType;
            }
            db.Update(product);
            db.SaveChanges();
            return new();
        }
        public async Task<ApplicationResponse> UpdateProductOfCurrentUserShopAsync(int productId, string? name,
        int? unitId, double? mass, double? pricePerMass,
        double? percentForFamiliarCustomer, double? percentForCustomer, double? priceForFamiliarCustomer,
        double? priceForCustomer, bool? hasAutoCalculatePermission,
        List<int>? categoryIds, List<Tuple<int, double>>? warehouses)
        {
            var response = await GetProductOfCurrentUserShop(productId);
            if (response.Result == null) return new ApplicationResponse { Error = response.Error };
            var product = response.Result;


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

                for (int i = 0; i < categoryIds.Count; i++)
                {
                    int categoryId = categoryIds[i];
                    bool isExist = await db.Set<ProductCategory>().AnyAsync(e => e.ShopId == product.ShopId && e.Id == categoryId);
                    if (!isExist)
                    {
                        return new(ApplicationError.CreateInvalidError($"CategoryIds[{i}]"));

                    }
                }

                var productCategoryProducts = await db.Set<ProductCategoryProduct>().Where(e => e.ProductId == product.Id).ToListAsync();
                db.RemoveRange(productCategoryProducts);
                db.SaveChanges();
                foreach (int categoryId in categoryIds)
                {
                    db.Set<ProductCategoryProduct>().Add(new ProductCategoryProduct
                    {
                        ProductId = product.Id,
                        ProductCategoryId = categoryId
                    });
                }
                db.SaveChanges();


            }
            if (warehouses != null)
            {

                for (int i = 0; i < warehouses.Count; i++)
                {
                    int warehouseId = warehouses[i].Item1;
                    bool isExist = await db.Set<Warehouse>().AnyAsync(e => e.ShopId == product.ShopId && e.Id == warehouseId);
                    if (!isExist)
                    {
                        return new(ApplicationError.CreateInvalidError($"Warehouses[{i}].WarehouseId"));

                    }
                }

                foreach (Tuple<int, double> tuple in warehouses)
                {
                    db.CreateOrUpdate(new WarehouseProduct
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
            db.Update(product);
            db.SaveChanges();
            return new();
        }

        public async Task<ApplicationResponse> AddPricePerMassOfCurrentUserShopAsync(List<int> categoryIds, double amountOfCash)
        {
            Shop? shop = await shopService.GetShopByCurrentUserIdAsync();
            if (shop == null)
            {
                return new(ApplicationError.CreateNotFoundError("Shop"));
            }
            Unit? cashUnit = shop.CashUnit;
            if (cashUnit == null)
            {
                return new(ApplicationError.CreateNotFoundError("CashUnit"));

            }
            var products = db.Set<Product>().Where(e => e.ShopId == shop.Id && e.HasAutoCalculatePermission && e.ProductCategoryProducts != null && e.ProductCategoryProducts.Any(c => categoryIds.Contains(c.ProductCategoryId))).ToList();
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
                db.Update(product);
            }
            db.SaveChanges();
            return new();
        }

        public async Task<ApplicationResponse> SoftlyDeleteProductOfCurrentUserShopAsync(int id)
        {
            var response = await GetProductOfCurrentUserShop(id);
            if (response.Result == null) return new ApplicationResponse { Error = response.Error };
            var product = response.Result;
            db.SoftDelete(product);
            return new();

        }
    }
}
