

using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;
using HardwareShop.Dal;
using HardwareShop.Dal.Models;

namespace HardwareShop.WebApi.Extensions
{
    public static class WebApplicationExtension
    {
        public static void SeedData(this WebApplication app)
        {
            var services = app.Services;
            const string assetFolder = "SampleImages";
            const string productAssetFile = "ProductAsset.jpg";
            const string shopAssetFile = "ShopAsset.jpg";
            const string userAssetFile = "UserAsset.jpg";
            using (IServiceScope scope = services.CreateScope())
            {
                IWebHostEnvironment? env = scope.ServiceProvider.GetService<IWebHostEnvironment>();
                if (env == null) return;
                if (!env.IsDevelopment())
                {
                    return;
                }
                IHashingPasswordService hashingPasswordService = scope.ServiceProvider.GetRequiredService<IHashingPasswordService>();
                using (var db = scope.ServiceProvider.GetRequiredService<MainDatabaseContext>())
                {
                    if (!db.Users.Any())
                    {
                        UnitCategory unitCategory = new UnitCategory()
                        {
                            Name = UnitCategoryConstants.MassCategoryName
                        };
                        UnitCategory unitCategory2 = new UnitCategory()
                        {
                            Name = UnitCategoryConstants.CurrencyCategoryName
                        };
                        db.UnitCategories.Add(unitCategory);
                        db.UnitCategories.Add(unitCategory2);
                        db.SaveChanges();

                        Unit unit = new Unit { Name = "Kg", UnitCategory = unitCategory };
                        Unit unit1 = new Unit { Name = "VND", UnitCategory = unitCategory2, StepNumber = 500 };
                        db.Units.Add(unit);
                        db.Units.Add(unit1);
                        db.SaveChanges();

                        var productAssetPath = Path.Join(assetFolder, productAssetFile);
                        var productAssetBytes = File.ReadAllBytes(productAssetPath);

                        var shopAssetPath = Path.Join(assetFolder, shopAssetFile);
                        var shopAssetBytes = File.ReadAllBytes(shopAssetPath);

                        var userAssetPath = Path.Join(assetFolder, userAssetFile);
                        var userAssetBytes = File.ReadAllBytes(userAssetPath);

                        var user = new User
                        {
                            Email = "huynhthehainam@gmail.com",
                            HashedPassword = hashingPasswordService.Hash("123"),
                            Phone = "+84967044037",
                            FirstName = "Nam",
                            LastName = "Huỳnh",
                            Role = HardwareShop.Core.Models.SystemUserRole.Admin,
                            Username = "admin",
                            Assets = new UserAsset[]
                            {
                            new UserAsset
                            {
                                AssetType = UserAssetConstants.AvatarAssetType,
                                Filename = userAssetFile,
                                Bytes =  userAssetBytes,
                                ContentType = ContentTypeConstants.JpegContentType
                            }
                            }
                        };
                        var user1 = new User
                        {
                            Email = "huynhthehainam.mismart@gmail.com",
                            HashedPassword = hashingPasswordService.Hash("123"),
                            Phone = "+84967044037",
                            FirstName = "Nam",
                            LastName = "Huỳnh",
                            Role = HardwareShop.Core.Models.SystemUserRole.Admin,
                            Username = "admin1",
                            Assets = new UserAsset[]
                            {
                            new UserAsset
                            {
                                AssetType = UserAssetConstants.AvatarAssetType,
                                Filename = userAssetFile,
                                Bytes =  userAssetBytes,
                                ContentType = ContentTypeConstants.JpegContentType
                            }
                            }
                        };
                        db.Users.Add(user);
                        db.Users.Add(user1);
                        db.SaveChanges();



                        var shop = new Shop
                        {
                            Name = "Admin shop",
                            Address = "123",
                            CashUnit = unit1,
                            Assets = new ShopAsset[]
                            {
                            new ShopAsset
                            {
                                AssetType = ShopAssetConstants.LogoAssetType,
                                Bytes = shopAssetBytes,
                                Filename = productAssetFile,
                                ContentType = ContentTypeConstants.JpegContentType
                            }
                            },
                            UserShops = new UserShop[]
                            {
                            new UserShop
                            {
                                UserId =  user.Id,
                                Role  = UserShopRole.Admin,

                            },
                            new UserShop
                            {
                                UserId =  user1.Id,
                                Role  = UserShopRole.Staff,

                            }
                            },
                            Customers = new Customer[] {
                                new Customer{
                                    Address="HCM",
                                    Name = "Nam",
                                    Phone = "+84967044037",

                                }
                            }

                        };

                        db.Shops.Add(shop);
                        db.SaveChanges();

                        var product = new Product
                        {
                            Name = "H13x26",
                            Mass = 2.5,
                            Unit = unit,
                            PercentForCustomer = 8,
                            PriceForCustomer = 12000,
                            ShopId = shop.Id,
                            PercentForFamiliarCustomer = 6,
                            PriceForFamiliarCustomer = 11000,
                            PricePerMass = 600,
                            ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {
                                                    Bytes = productAssetBytes,
                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                                    Filename = productAssetFile,
                                                    ContentType= ContentTypeConstants.JpegContentType
                                                }
                                            }
                        };
                        var product2 = new Product
                        {
                            Name = "H20x40",
                            Mass = 2.5,
                            Unit = unit,
                            PercentForCustomer = 8,
                            PriceForCustomer = 12000,
                            ShopId = shop.Id,
                            PercentForFamiliarCustomer = 6,
                            PriceForFamiliarCustomer = 11000,
                            PricePerMass = 600,
                            ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {
                                                    Bytes = productAssetBytes,
                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                                    Filename = productAssetFile,
                                                    ContentType= ContentTypeConstants.JpegContentType
                                                }
                                            }
                        };
                        var product3 = new Product
                        {
                            Name = "H30x60",
                            Mass = 2.5,
                            Unit = unit,
                            PercentForCustomer = 8,
                            PriceForCustomer = 12000,
                            ShopId = shop.Id,
                            PercentForFamiliarCustomer = 6,
                            PriceForFamiliarCustomer = 11000,
                            PricePerMass = 600,
                            ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {
                                                    Bytes = productAssetBytes,
                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                                    Filename = productAssetFile,
                                                    ContentType= ContentTypeConstants.JpegContentType
                                                }
                                            }
                        };
                        var product4 = new Product
                        {
                            Name = "V4",
                            Mass = 2.5,
                            Unit = unit,
                            PercentForCustomer = 8,
                            PriceForCustomer = 12000,
                            ShopId = shop.Id,
                            PercentForFamiliarCustomer = 6,
                            PriceForFamiliarCustomer = 11000,
                            PricePerMass = 600,
                            ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {
                                                    Bytes = productAssetBytes,
                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                                    Filename = productAssetFile,
                                                    ContentType= ContentTypeConstants.JpegContentType
                                                }
                                            }
                        };
                        var product5 = new Product
                        {
                            Name = "V6",
                            Mass = 2.5,
                            Unit = unit,
                            PercentForCustomer = 8,
                            PriceForCustomer = 12000,
                            ShopId = shop.Id,
                            PercentForFamiliarCustomer = 6,
                            PriceForFamiliarCustomer = 11000,
                            PricePerMass = 600,
                            ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {
                                                    Bytes = productAssetBytes,
                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                                    Filename = productAssetFile,
                                                    ContentType= ContentTypeConstants.JpegContentType
                                                }
                                            }
                        };
                        var product6 = new Product
                        {
                            Name = "V3",
                            Mass = 2.5,
                            Unit = unit,
                            PercentForCustomer = 8,
                            PriceForCustomer = 12000,
                            ShopId = shop.Id,
                            PercentForFamiliarCustomer = 6,
                            PriceForFamiliarCustomer = 11000,
                            PricePerMass = 600,
                            ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {
                                                    Bytes = productAssetBytes,
                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                                    Filename = productAssetFile,
                                                    ContentType= ContentTypeConstants.JpegContentType
                                                }
                                            }
                        };
                        db.Products.Add(product);
                        db.Products.Add(product2);
                        db.Products.Add(product3);
                        db.Products.Add(product4);
                        db.Products.Add(product5);
                        db.Products.Add(product6);
                        db.SaveChanges();
                        var productCategory = new ProductCategory
                        {
                            Name = "Hoa Sen",
                            Description = "Hoa Sen",
                            ShopId = shop.Id,
                        };
                        var productCategory2 = new ProductCategory
                        {
                            Name = "Tôn",
                            Description = "Tôn",
                            ShopId = shop.Id,
                        };
                        db.ProductCategories.Add(productCategory);
                        db.ProductCategories.Add(productCategory2);
                        db.SaveChanges();
                        ProductCategoryProduct productCategoryProduct = new ProductCategoryProduct
                        {
                            Product = product,
                            ProductCategory = productCategory,
                        };
                        ProductCategoryProduct productCategoryProduct2 = new ProductCategoryProduct
                        {
                            Product = product,
                            ProductCategory = productCategory2,
                        };
                        db.ProductCategoryProducts.Add(productCategoryProduct);
                        db.ProductCategoryProducts.Add(productCategoryProduct2);
                        db.SaveChanges();
                    }
                }

            }
        }
    }
}