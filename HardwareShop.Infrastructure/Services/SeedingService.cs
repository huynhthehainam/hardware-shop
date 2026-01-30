

using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Enums;
using HardwareShop.Domain.Models;
using HardwareShop.Infrastructure.Data;
using HardwareShop.Infrastructure.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;


namespace HardwareShop.Infrastructure.Services
{
    public class KeycloakUser
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public bool EmailVerified { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
    public class KeycloakCredential
    {
        public string Type { get; set; } = "password";
        public string Value { get; set; } = default!;
        public bool Temporary { get; set; } = false;
    }
    public class KeycloakRealm
    {
        public string Realm { get; set; } = default!;
        public bool Enabled { get; set; } = true;
    }

    public class KeycloakClient
    {
        public string ClientId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool Enabled { get; set; } = true;

        public bool PublicClient { get; set; } = true;
        public bool StandardFlowEnabled { get; set; } = true;
        public bool ImplicitFlowEnabled { get; set; } = false;
        public bool DirectAccessGrantsEnabled { get; set; } = true;

        public string RootUrl { get; set; } = default!;
        public string BaseUrl { get; set; } = default!;
        public string AdminUrl { get; set; } = default!;

        public List<string> RedirectUris { get; set; } = [];
        public List<string> WebOrigins { get; set; } = [];

        public Dictionary<string, string> Attributes { get; set; } = [];
    }

    public class SeedingService : ISeedingService
    {

        private readonly MainDatabaseContext db;
        private readonly IHashingPasswordService hashingPasswordService;
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        public SeedingService(MainDatabaseContext db, IHashingPasswordService hashingPasswordService, IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            this.db = db;
            this.hashingPasswordService = hashingPasswordService;
            this.configuration = configuration;
            this.httpClient = httpClientFactory.CreateClient();
        }


        private async Task<string> GetAdminAccessTokenAsync()
        {
            var keycloakUrl = configuration["Keycloak:Url"] ?? "http://localhost:8081";
            var adminUser = configuration["Keycloak:AdminUser"] ?? "admin";
            var adminPassword = configuration["Keycloak:AdminPassword"] ?? "admin";

            var response = await httpClient.PostAsync(
                $"{keycloakUrl}/realms/master/protocol/openid-connect/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["username"] = adminUser,
                    ["password"] = adminPassword,
                    ["grant_type"] = "password",
                    ["client_id"] = "admin-cli"
                })
            );

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("access_token").GetString()!;
        }
        public async Task EnsureRealmExistsAsync(string realm)
        {
            var keycloakUrl = configuration["Keycloak:Url"] ?? "http://localhost:8081";
            var token = await GetAdminAccessTokenAsync();

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Check realm
            var check = await httpClient.GetAsync($"{keycloakUrl}/admin/realms/{realm}");
            if (check.IsSuccessStatusCode)
                return;

            if (check.StatusCode != System.Net.HttpStatusCode.NotFound)
                check.EnsureSuccessStatusCode();

            // Create realm
            var createResponse = await httpClient.PostAsJsonAsync(
                $"{keycloakUrl}/admin/realms",
                new KeycloakRealm
                {
                    Realm = realm,
                    Enabled = true
                });

            createResponse.EnsureSuccessStatusCode();
        }
        public async Task<string> EnsureUserExistsAsync(string realm)
        {
            var keycloakUrl = configuration["Keycloak:Url"] ?? "http://localhost:8081";
            var token = await GetAdminAccessTokenAsync();

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            const string username = "namhuynh";

            // 1. Check user exists
            var usersResponse = await httpClient.GetAsync(
                $"{keycloakUrl}/admin/realms/{realm}/users?username={username}&exact=true"
            );

            usersResponse.EnsureSuccessStatusCode();

            var users = await usersResponse.Content.ReadFromJsonAsync<List<JsonElement>>();
            if (users is { Count: > 0 })
                return users![0].GetProperty("id").GetString()!;

            // 2. Create user
            var createUserResponse = await httpClient.PostAsJsonAsync(
                $"{keycloakUrl}/admin/realms/{realm}/users",
                new KeycloakUser
                {
                    Username = username,
                    Email = "huynhthehainam@gmail.com",
                    FirstName = "nam",
                    LastName = "huynh",
                    Enabled = true,
                    EmailVerified = true
                });

            createUserResponse.EnsureSuccessStatusCode();

            // 3. Fetch created user ID
            var createdUsersResponse = await httpClient.GetAsync(
                $"{keycloakUrl}/admin/realms/{realm}/users?username={username}&exact=true"
            );

            createdUsersResponse.EnsureSuccessStatusCode();

            var createdUsers =
                await createdUsersResponse.Content.ReadFromJsonAsync<List<JsonElement>>();

            var userId = createdUsers![0].GetProperty("id").GetString();

            // 4. Set password (no required action)
            var passwordResponse = await httpClient.PutAsJsonAsync(
                $"{keycloakUrl}/admin/realms/{realm}/users/{userId}/reset-password",
                new KeycloakCredential
                {
                    Value = "admin123456",
                    Temporary = false
                });

            passwordResponse.EnsureSuccessStatusCode();
            return userId!;
        }

        public async Task EnsureClientExistsAsync(string realm)
        {
            var keycloakUrl = configuration["Keycloak:Url"] ?? "http://localhost:8081";
            var token = await GetAdminAccessTokenAsync();

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            const string clientId = "hardware-shop-angular";

            // Check client
            var clientsResponse = await httpClient.GetAsync(
                $"{keycloakUrl}/admin/realms/{realm}/clients?clientId={clientId}"
            );

            clientsResponse.EnsureSuccessStatusCode();

            var existingClients =
                await clientsResponse.Content.ReadFromJsonAsync<List<JsonElement>>();

            if (existingClients is { Count: > 0 })
                return;

            // Create client
            var client = new KeycloakClient
            {
                ClientId = clientId,
                Name = clientId,

                RootUrl = "http://localhost:4200",
                BaseUrl = "http://localhost:4200",
                AdminUrl = "http://localhost:4200",

                RedirectUris =
     [
         "http://localhost:4200/*",
        "https://oauth.pstmn.io/v1/callback"
     ],

                WebOrigins =
     [
         "http://localhost:4200"
     ],

                Attributes =
    {
        ["post.logout.redirect.uris"] = "http://localhost:4200/*"
    }
            };


            var createResponse = await httpClient.PostAsJsonAsync(
                $"{keycloakUrl}/admin/realms/{realm}/clients",
                client
            );
            var content = await createResponse.Content.ReadAsStringAsync();
            createResponse.EnsureSuccessStatusCode();
        }

        public async Task SeedDataAsync(string firstUserId)
        {
            const string assetFolder = "SampleImages";
            const string productAssetFile = "ProductAsset.jpg";
            const string shopAssetFile = "ShopAsset.jpg";
            const string userAssetFile = "UserAsset.jpg";
            const string user2AssetFile = "UserAsset2.png";
            const string user3AssetFile = "UserAsset3.png";
            const string countryAssetFile = "CountryAsset.png";
            const string countryAsset2File = "CountryAsset2.png";
            var user = await db.Users.FirstOrDefaultAsync(e => e.Id == Guid.Parse(firstUserId));
            if (user == null)
            {
                user = await db.Users.FirstOrDefaultAsync();
                if (user != null)
                {
                    user.Id = Guid.Parse(firstUserId);
                    db.Users.Update(user);
                    await db.SaveChangesAsync();
                }
            }

            if (user is null)
            {
                string productAssetPath = System.IO.Path.Join(assetFolder, productAssetFile);
                byte[] productAssetBytes = File.ReadAllBytes(productAssetPath);

                string shopAssetPath = System.IO.Path.Join(assetFolder, shopAssetFile);
                byte[] shopAssetBytes = File.ReadAllBytes(shopAssetPath);

                string userAssetPath = System.IO.Path.Join(assetFolder, userAssetFile);
                byte[] userAssetBytes = File.ReadAllBytes(userAssetPath);

                string user2AssetPath = System.IO.Path.Join(assetFolder, user2AssetFile);
                byte[] user2AssetBytes = File.ReadAllBytes(user2AssetPath);

                string user3AssetPath = System.IO.Path.Join(assetFolder, user3AssetFile);
                byte[] user3AssetBytes = File.ReadAllBytes(user3AssetPath);

                string countryAssetPath = System.IO.Path.Join(assetFolder, countryAssetFile);
                byte[] countryAssetBytes = File.ReadAllBytes(countryAssetPath);

                string countryAsset2Path = System.IO.Path.Join(assetFolder, countryAsset2File);
                byte[] countryAsset2Bytes = File.ReadAllBytes(countryAsset2Path);

                var productAsset = new Asset()
                {

                    Bytes = productAssetBytes,
                    FileName = productAssetFile,
                    ContentType = ContentTypeConstants.PngContentType
                };
                var shopAsset = new Asset()
                {
                    Bytes = shopAssetBytes,
                    FileName = shopAssetFile,
                    ContentType = ContentTypeConstants.PngContentType
                };
                var userAsset = new Asset()
                {
                    Bytes = userAssetBytes,
                    FileName = userAssetFile,
                    ContentType = ContentTypeConstants.JpegContentType
                };
                var user2Asset = new Asset()
                {
                    Bytes = user2AssetBytes,
                    FileName = user2AssetFile,
                    ContentType = ContentTypeConstants.PngContentType
                };
                var user3Asset = new Asset()
                {
                    Bytes = user3AssetBytes,
                    FileName = user3AssetFile,
                    ContentType = ContentTypeConstants.PngContentType
                };
                var countryAsset = new Asset()
                {
                    Bytes = countryAssetBytes,
                    FileName = countryAssetFile,
                    ContentType = ContentTypeConstants.PngContentType
                };
                var country2Asset = new Asset()
                {
                    Bytes = countryAsset2Bytes,
                    FileName = countryAsset2File,
                    ContentType = ContentTypeConstants.PngContentType
                };

                db.Assets.AddRange(new Asset[] { productAsset, shopAsset, userAsset, user2Asset, user3Asset, countryAsset, country2Asset });
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



                User newUser = new()
                {
                    Id = Guid.Parse(firstUserId),
                    Phone = "+84967044037",

                    PhoneCountryId = country.Id,
                    SecretValue = "123",
                    Assets = new UserAsset[]
                    {
                            new UserAsset
                            {
                                AssetType = UserAssetConstants.AvatarAssetType,
                               Asset =userAsset,
                            }
                    }
                };

                _ = db.Users.Add(newUser);
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
                                UserId =  newUser.Id,
                                Role  = UserShopRole.Admin,

                            },

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

        public async Task EnsureKafkaTopicsExistAsync()
        {
            var kafkaSection = configuration.GetSection("Kafka");

            var bootstrapServers = kafkaSection.GetValue<string>("BootstrapServers");

            if (string.IsNullOrWhiteSpace(bootstrapServers))
                throw new InvalidOperationException("Kafka BootstrapServers is not configured.");

            var adminConfig = new AdminClientConfig
            {
                BootstrapServers = bootstrapServers
            };

            using var adminClient = new AdminClientBuilder(adminConfig).Build();

            // All saga topics
            var requiredTopics = new[]
            {
                BookingSagaTopics.FlightBook,
                BookingSagaTopics.FlightBooked,
                BookingSagaTopics.FlightFailed,
                BookingSagaTopics.HotelBook,
                BookingSagaTopics.HotelBooked,
                BookingSagaTopics.HotelFailed,
                BookingSagaTopics.FlightCancel,
                BookingSagaTopics.FlightCancelled,
                BookingSagaTopics.BookingDLQ
            };

            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));

                var existingTopics = metadata.Topics
                    .Select(t => t.Topic)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var topicsToCreate = requiredTopics
                    .Where(t => !existingTopics.Contains(t))
                    .Select(t => new TopicSpecification
                    {
                        Name = t,
                        NumPartitions = 3,
                        ReplicationFactor = 1
                    })
                    .ToList();

                if (!topicsToCreate.Any())
                {
                    Console.WriteLine("All Kafka saga topics already exist.");
                    return;
                }

                await adminClient.CreateTopicsAsync(
                    topicsToCreate,
                    new CreateTopicsOptions
                    {
                        RequestTimeout = TimeSpan.FromSeconds(15)
                    });

                foreach (var topic in topicsToCreate)
                {
                    Console.WriteLine($"Kafka topic '{topic.Name}' created.");
                }
            }
            catch (CreateTopicsException ex)
            {
                foreach (var result in ex.Results)
                {
                    if (result.Error.Code == ErrorCode.TopicAlreadyExists)
                    {
                        Console.WriteLine($"Kafka topic '{result.Topic}' already exists.");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Failed to create topic '{result.Topic}': {result.Error.Reason}");
                        throw;
                    }
                }
            }
        }
    }
}