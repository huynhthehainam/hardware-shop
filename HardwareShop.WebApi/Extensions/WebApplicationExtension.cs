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
            const string countryAssetFile = "CountryAsset.png";
            const string countryAsset2File = "CountryAsset2.png";
            using IServiceScope scope = services.CreateScope();
            IWebHostEnvironment? env = scope.ServiceProvider.GetService<IWebHostEnvironment>();
            if (env == null) return;
            if (!env.IsDevelopment())
            {
                return;
            }
            IHashingPasswordService hashingPasswordService = scope.ServiceProvider.GetRequiredService<IHashingPasswordService>();
            using var db = scope.ServiceProvider.GetRequiredService<MainDatabaseContext>();
            if (!db.Users.Any())
            {
                var productAssetPath = Path.Join(assetFolder, productAssetFile);
                var productAssetBytes = File.ReadAllBytes(productAssetPath);

                var shopAssetPath = Path.Join(assetFolder, shopAssetFile);
                var shopAssetBytes = File.ReadAllBytes(shopAssetPath);

                var userAssetPath = Path.Join(assetFolder, userAssetFile);
                var userAssetBytes = File.ReadAllBytes(userAssetPath);

                var countryAssetPath = Path.Join(assetFolder, countryAssetFile);
                var countryAssetBytes = File.ReadAllBytes(countryAssetPath);

                var countryAsset2Path = Path.Join(assetFolder, countryAsset2File);
                var countryAsset2Bytes = File.ReadAllBytes(countryAsset2Path);


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
                db.UnitCategories.Add(massCategory);
                db.UnitCategories.Add(singleCategory);
                db.UnitCategories.Add(currencyCategory);
                db.UnitCategories.Add(lengthCategory);
                db.UnitCategories.Add(volumeCategory);
                db.SaveChanges();

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

                db.Units.AddRange(new Unit[]{
                    unit, unit1, unit2,unit3, unit4, unit5
                });

                db.SaveChanges();


                var country = new Country()
                {
                    Name = "Vietnam",
                    PhonePrefix = "+84",
                    Asset = new CountryAsset()
                    {
                        AssetType = CountryAssetConstants.IconType,
                        Bytes = countryAssetBytes,
                        Filename = countryAssetFile,
                        ContentType = ContentTypeConstants.PngContentType
                    }
                };

                var country2 = new Country()
                {
                    Name = "China",
                    PhonePrefix = "+86",
                    Asset = new CountryAsset()
                    {
                        AssetType = CountryAssetConstants.IconType,
                        Bytes = countryAsset2Bytes,
                        Filename = countryAsset2File,
                        ContentType = ContentTypeConstants.PngContentType
                    }
                };
                db.Countries.Add(country);
                db.Countries.Add(country2);
                db.SaveChanges();



                var user = new User
                {
                    Email = "huynhthehainam@gmail.com",
                    HashedPassword = hashingPasswordService.Hash("123"),
                    Phone = "967044037",
                    FirstName = "Nam",
                    LastName = "Huỳnh",
                    Role = HardwareShop.Core.Models.SystemUserRole.Admin,
                    Username = "admin",
                    PhoneCountryId = country.Id,
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
                    Address = "850 Xa lộ Hà Nội, Thủ Đức, HCM",
                    CashUnit = unit1,
                    Phones = new string[] { "+84909933033", "+84933933033" },
                    PhoneOwners = new string[] { "C. Hải", "A. Cường" },
                    Warehouses = new Warehouse[]{
                        new Warehouse(){
                            Name = "Kho 1",
                            Address  = "Châu Đức, BRVT",
                        }
                    },
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
                                    Phone = "967044037",
                                    PhoneCountryId= country.Id,
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
                    OriginalPrice = 10000,
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
                    OriginalPrice = 10000,
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
                    OriginalPrice = 10000,
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
                    OriginalPrice = 10000,
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
                    OriginalPrice = 10000,
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
                    OriginalPrice = 10000,
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
                db.ProductCategoryProducts.Add(productCategoryProduct);
                db.ProductCategoryProducts.Add(productCategoryProduct2);
                db.SaveChanges();
            }
        }
    }
}