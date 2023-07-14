using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.WebApi.Extensions
{
    public static class WebApplicationExtension
    {
        public static void SeedData(this WebApplication app)
        {
            IServiceProvider services = app.Services;
            const string assetFolder = "SampleImages";
            const string productAssetFile = "ProductAsset.jpg";
            const string shopAssetFile = "ShopAsset.jpg";
            const string userAssetFile = "UserAsset.jpg";
            const string countryAssetFile = "CountryAsset.png";
            const string countryAsset2File = "CountryAsset2.png";
            using IServiceScope scope = services.CreateScope();
            IWebHostEnvironment? env = scope.ServiceProvider.GetService<IWebHostEnvironment>();
            if (env == null)
            {
                return;
            }
            IHashingPasswordService hashingPasswordService = scope.ServiceProvider.GetRequiredService<IHashingPasswordService>();

            using MainDatabaseContext db = scope.ServiceProvider.GetRequiredService<MainDatabaseContext>();
            if (!env.IsDevelopment())
            {
                db.Database.Migrate();
            }
            if (!db.Users.Any())
            {
                string productAssetPath = System.IO.Path.Join(assetFolder, productAssetFile);
                byte[] productAssetBytes = File.ReadAllBytes(productAssetPath);

                string shopAssetPath = System.IO.Path.Join(assetFolder, shopAssetFile);
                byte[] shopAssetBytes = File.ReadAllBytes(shopAssetPath);

                string userAssetPath = System.IO.Path.Join(assetFolder, userAssetFile);
                byte[] userAssetBytes = File.ReadAllBytes(userAssetPath);

                string countryAssetPath = System.IO.Path.Join(assetFolder, countryAssetFile);
                byte[] countryAssetBytes = File.ReadAllBytes(countryAssetPath);

                string countryAsset2Path = System.IO.Path.Join(assetFolder, countryAsset2File);
                byte[] countryAsset2Bytes = File.ReadAllBytes(countryAsset2Path);

                var productAsset = new Asset()
                {

                    Bytes = productAssetBytes,
                    Filename = productAssetFile,
                    ContentType = ContentTypeConstants.PngContentType
                };
                var shopAsset = new Asset()
                {
                    Bytes = shopAssetBytes,
                    Filename = shopAssetFile,
                    ContentType = ContentTypeConstants.PngContentType
                };
                var userAsset = new Asset()
                {
                    Bytes = userAssetBytes,
                    Filename = userAssetFile,
                    ContentType = ContentTypeConstants.PngContentType
                };
                var countryAsset = new Asset()
                {
                    Bytes = countryAssetBytes,
                    Filename = countryAssetFile,
                    ContentType = ContentTypeConstants.PngContentType
                };
                var country2Asset = new Asset()
                {
                    Bytes = countryAsset2Bytes,
                    Filename = countryAsset2File,
                    ContentType = ContentTypeConstants.PngContentType
                };

                db.Assets.AddRange(new Asset[] { productAsset, shopAsset, userAsset, countryAsset, country2Asset });
                db.SaveChanges();


                UnitCategory massCategory = new()
                {
                    Name = "Mass"
                };
                UnitCategory currencyCategory = new()
                {
                    Name = UnitCategoryConstants.CurrencyCategoryName
                };
                UnitCategory singleCategory = new()
                {
                    Name = "Single"
                };
                UnitCategory lengthCategory = new()
                {
                    Name = "Length"
                };
                UnitCategory volumeCategory = new()
                {
                    Name = "Volume"
                };
                _ = db.UnitCategories.Add(massCategory);
                _ = db.UnitCategories.Add(singleCategory);
                _ = db.UnitCategories.Add(currencyCategory);
                _ = db.UnitCategories.Add(lengthCategory);
                _ = db.UnitCategories.Add(volumeCategory);
                _ = db.SaveChanges();

                Unit unit = new()
                {
                    Name = "Kg",
                    UnitCategory = massCategory,
                    IsPrimary = true
                };
                Unit unit1 = new()
                {
                    Name = "VND",
                    UnitCategory = currencyCategory,
                    StepNumber = 500,
                    CompareWithPrimaryUnit = 23495
                };
                Unit unit2 = new()
                {
                    Name = "USD",
                    UnitCategory = currencyCategory,
                    StepNumber = 0.01,
                    IsPrimary = true
                };
                Unit unit3 = new()
                {
                    Name = "Piece",
                    UnitCategory = singleCategory,
                    CompareWithPrimaryUnit = 1,
                    IsPrimary = true,
                    StepNumber = 1,
                };
                Unit unit4 = new()
                {
                    Name = "Meter",
                    UnitCategory = lengthCategory,
                    StepNumber = 0.01,
                    CompareWithPrimaryUnit = 1,
                    IsPrimary = true,
                };
                Unit unit5 = new()
                {
                    Name = "Litter",
                    UnitCategory = volumeCategory,
                    StepNumber = 0.01,
                    CompareWithPrimaryUnit = 1,
                    IsPrimary = true,
                };
                Unit unit6 = new()
                {
                    Name = "Bar",
                    UnitCategory = singleCategory,
                    CompareWithPrimaryUnit = 1,
                    IsPrimary = false,
                    StepNumber = 1,
                };

                db.Units.AddRange(new Unit[]{
                    unit, unit1, unit2,unit3, unit4, unit5,unit6
                });

                _ = db.SaveChanges();


                Country country = new()
                {
                    Name = "Vietnam",
                    PhonePrefix = "+84",
                    Asset = new CountryAsset()
                    {
                        AssetType = CountryAssetConstants.IconType,
                        Asset = countryAsset
                    }
                };

                Country country2 = new()
                {
                    Name = "China",
                    PhonePrefix = "+86",
                    Asset = new CountryAsset()
                    {
                        AssetType = CountryAssetConstants.IconType,
                        Asset = country2Asset
                    }
                };
                _ = db.Countries.Add(country);
                _ = db.Countries.Add(country2);
                _ = db.SaveChanges();



                User user = new()
                {
                    Email = "huynhthehainam@gmail.com",
                    HashedPassword = hashingPasswordService.Hash("123"),
                    Phone = "967044037",
                    FirstName = "Nam",
                    LastName = "Huỳnh",
                    Role = SystemUserRole.Admin,
                    Username = "admin",
                    PhoneCountryId = country.Id,
                    Assets = new UserAsset[]
                    {
                            new UserAsset
                            {
                                AssetType = UserAssetConstants.AvatarAssetType,
                               Asset =userAsset,
                            }
                    }
                };
                User user1 = new()
                {
                    Email = "huynhthehainam.mismart@gmail.com",
                    HashedPassword = hashingPasswordService.Hash("123"),
                    Phone = "+84967044037",
                    FirstName = "Nam",
                    LastName = "Huỳnh",
                    Role = SystemUserRole.Admin,
                    Username = "admin1",
                    Assets = new UserAsset[]
                    {
                            new UserAsset
                            {
                                AssetType = UserAssetConstants.AvatarAssetType,
                            Asset = userAsset,
                            }
                    }
                };
                User user2 = new()
                {
                    Email = "huynhthehainam.mismart@gmail.com",
                    HashedPassword = hashingPasswordService.Hash("123"),
                    Phone = "+84967044037",
                    FirstName = "Nam",
                    LastName = "Huỳnh",
                    Role = SystemUserRole.Admin,
                    Username = "admin2",
                    Assets = new UserAsset[]
                   {
                            new UserAsset
                            {
                                AssetType = UserAssetConstants.AvatarAssetType,
                            Asset = userAsset,
                            }
                   }
                };
                _ = db.Users.Add(user);
                _ = db.Users.Add(user1);
                _ = db.Users.Add(user2);
                _ = db.SaveChanges();



                Shop shop = new()
                {
                    Name = "Admin shop",
                    Address = "850 Xa lộ Hà Nội, Thủ Đức, HCM",
                    CashUnit = unit1,
                    ShopSetting = new ShopSetting()
                    {
                        IsAllowedToShowInvoiceDownloadOptions = true
                    },
                    Phones = new ShopPhone[]{
                        new ShopPhone(){
                            CountryId = country.Id,
                            OwnerName = "A. Cường",
                            Phone = "909933033"
                        },
                         new ShopPhone(){
                             CountryId = country.Id,
                            OwnerName = "C. Hải",
                            Phone = "933933033"
                         }
                    },
                    Assets = new ShopAsset[]
                    {
                            new ShopAsset
                            {
                                AssetType = ShopAssetConstants.LogoAssetType,
                               Asset =shopAsset,
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

                            },
                               new UserShop
                            {
                                UserId =  user2.Id,
                                Role  = UserShopRole.Staff,

                            }
                    },
                    Customers = new Customer[] {
                                new Customer{
                                    Address="HCM",
                                    Name = "Nam",
                                    Phone = "967044037",
                                    PhoneCountryId= country.Id,
                                }
                            }

                };

                _ = db.Shops.Add(shop);
                _ = db.SaveChanges();
                if (env.IsDevelopment())
                {
                    Product product = new()
                    {
                        Name = "H13x26",
                        Mass = 2.5,
                        Unit = unit6,
                        PercentForCustomer = 8,
                        PriceForCustomer = 12000,
                        ShopId = shop.Id,
                        OriginalPrice = 10000,
                        PercentForFamiliarCustomer = 6,
                        PriceForFamiliarCustomer = 11000,
                        PricePerMass = 600,
                        ProductAssets = new ProductAsset[]{
                        new ProductAsset
                        {

                            AssetType =  ProductAssetConstants.ThumbnailAssetType,
                           Asset =productAsset,
                        }
                    }
                    };
                    Product product2 = new()
                    {
                        Name = "H20x40",
                        Mass = 2.5,
                        Unit = unit6,
                        PercentForCustomer = 8,
                        PriceForCustomer = 12000,
                        ShopId = shop.Id,
                        OriginalPrice = 10000,
                        PercentForFamiliarCustomer = 6,
                        PriceForFamiliarCustomer = 11000,
                        PricePerMass = 600,
                        ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {

                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                               Asset =productAsset,
                                                }
                                            }
                    };
                    Product product3 = new()
                    {
                        Name = "H30x60",
                        Mass = 2.5,
                        Unit = unit6,
                        PercentForCustomer = 8,
                        PriceForCustomer = 12000,
                        ShopId = shop.Id,
                        OriginalPrice = 10000,
                        PercentForFamiliarCustomer = 6,
                        PriceForFamiliarCustomer = 11000,
                        PricePerMass = 600,
                        ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {

                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                               Asset =productAsset,
                                                }
                                            }
                    };
                    Product product4 = new()
                    {
                        Name = "V4",
                        Mass = 2.5,
                        Unit = unit6,
                        PercentForCustomer = 8,
                        PriceForCustomer = 12000,
                        ShopId = shop.Id,
                        OriginalPrice = 10000,
                        PercentForFamiliarCustomer = 6,
                        PriceForFamiliarCustomer = 11000,
                        PricePerMass = 600,
                        ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {

                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                                   Asset =productAsset,
                                                }
                                            }
                    };
                    Product product5 = new()
                    {
                        Name = "V6",
                        Mass = 2.5,
                        Unit = unit6,
                        PercentForCustomer = 8,
                        PriceForCustomer = 12000,
                        OriginalPrice = 10000,
                        ShopId = shop.Id,
                        PercentForFamiliarCustomer = 6,
                        PriceForFamiliarCustomer = 11000,
                        PricePerMass = 600,
                        ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {

                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                                 Asset =productAsset,
                                                }
                                            }
                    };
                    Product product6 = new()
                    {
                        Name = "V3",
                        Mass = 2.5,
                        Unit = unit6,
                        PercentForCustomer = 8,
                        PriceForCustomer = 12000,
                        OriginalPrice = 10000,
                        ShopId = shop.Id,
                        PercentForFamiliarCustomer = 6,
                        PriceForFamiliarCustomer = 11000,
                        PricePerMass = 600,
                        ProductAssets = new ProductAsset[]{
                                                new ProductAsset
                                                {

                                                    AssetType =  ProductAssetConstants.ThumbnailAssetType,
                                                  Asset =productAsset,
                                                }
                                            }
                    };
                    _ = db.Products.Add(product);
                    _ = db.Products.Add(product2);
                    _ = db.Products.Add(product3);
                    _ = db.Products.Add(product4);
                    _ = db.Products.Add(product5);
                    _ = db.Products.Add(product6);
                    _ = db.SaveChanges();


                    Warehouse warehouse1 = new()
                    {
                        Name = "Kho 1",
                        Address = "Châu Đức, BRVT",
                        ShopId = shop.Id,
                        WarehouseProducts = new WarehouseProduct[]{
                        new WarehouseProduct(){
                            ProductId = product.Id,
                            Quantity = 200,
                        },
                        new WarehouseProduct(){
                            ProductId=product2.Id,
                            Quantity = 200,
                        },
                        new WarehouseProduct(){
                            ProductId = product3.Id,
                            Quantity = 300,
                        },
                        new WarehouseProduct(){
                            ProductId = product4.Id,
                            Quantity = 40.2,
                        },
                        new WarehouseProduct(){
                            ProductId = product5.Id,
                            Quantity = 32.1
                        }
                    }
                    };

                    _ = db.Warehouses.Add(warehouse1);
                    _ = db.SaveChanges();


                    ProductCategory productCategory = new()
                    {
                        Name = "Hoa Sen",
                        Description = "Hoa Sen",
                        ShopId = shop.Id,
                    };
                    ProductCategory productCategory2 = new()
                    {
                        Name = "Tôn",
                        Description = "Tôn",
                        ShopId = shop.Id,
                    };
                    _ = db.ProductCategories.Add(productCategory);
                    _ = db.ProductCategories.Add(productCategory2);
                    _ = db.SaveChanges();
                    ProductCategoryProduct productCategoryProduct = new()
                    {
                        Product = product,
                        ProductCategory = productCategory,
                    };
                    ProductCategoryProduct productCategoryProduct2 = new()
                    {
                        Product = product,
                        ProductCategory = productCategory2,
                    };
                    _ = db.ProductCategoryProducts.Add(productCategoryProduct);
                    _ = db.ProductCategoryProducts.Add(productCategoryProduct2);
                    _ = db.SaveChanges();
                }
            }
        }
    }
}