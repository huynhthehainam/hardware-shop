
using System.Text;
using HardwareShop.Business.Extensions;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Implementations;
using HardwareShop.Core.Services;
using HardwareShop.Dal;
using HardwareShop.Dal.Extensions;
using HardwareShop.Dal.Models;
using HardwareShop.WebApi.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace HardwareShop.WebApi;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var mainAllowSpecificOrigins = "mainAllowSpecificOrigins";
        var customCorsUrls = new List<string>() { "http://localhost:3000" };
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: mainAllowSpecificOrigins, builder =>
            {
                builder.WithOrigins(
                customCorsUrls.ToArray()
                ).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            });
        });



        builder.Services.AddControllers();
        builder.Services.AddEntityFrameworkNpgsql().AddDbContext<MainDatabaseContext>((sp, opt) => opt.UseNpgsql(builder.Configuration.GetConnectionString("AppConn"), b =>
        {
            b.MigrationsAssembly("HardwareShop.WebApi");
        }).UseInternalServiceProvider(sp));
        builder.Services.AddDistributedRedisCache(option =>
        {
            option.Configuration = builder.Configuration["RedisSettings:Host"] + ":" + builder.Configuration["RedisSettings:Port"] + ",connectTimeout=10000,syncTimeout=10000";
        });

        builder.Services.AddMvc().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        });


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
                },
                new string[] { }
            }
    });
        });



        var jwtConfiguration = builder.Configuration.GetSection("JwtConfiguration");
        builder.Services.Configure<AuthConfiguration>(jwtConfiguration);
        var appSettings = jwtConfiguration.Get<JwtConfiguration>();
        var key = Encoding.ASCII.GetBytes(appSettings.SecretKey ?? "");
        builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
           {
               x.Events = new JwtBearerEvents();
               x.Events.OnMessageReceived = (context) =>
               {
                   var bb = context.HttpContext.Request.Query["access_token"];
                   context.Token = context.HttpContext.Request.Query["access_token"];
                   return Task.CompletedTask;
               };

               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ClockSkew = TimeSpan.Zero,
                   ValidateLifetime = true,
                   LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken,
                                            TokenValidationParameters validationParameters) =>
                   {
                       Console.WriteLine("hello world");
                       return notBefore.HasValue ? notBefore.Value <= DateTime.UtcNow : true &&
                                  expires.HasValue ? expires.Value >= DateTime.UtcNow : true;
                   }
               };
           });

        #region Configuration
        builder.Services.Configure<HashingConfiguration>(builder.Configuration.GetSection("HashingConfiguration"));
        builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfiguration"));
        builder.Services.Configure<LanguageConfiguration>(builder.Configuration.GetSection("LanguageConfiguration"));

        #endregion
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<ILanguageService, LanguageService>();
        builder.Services.AddScoped<IResponseResultBuilder, ResponseResultBuilder>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddSingleton<IHashingPasswordService, HashingPasswordService>();
        builder.Services.ConfigureRepository();
        builder.Services.ConfigureBusiness();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseCors(mainAllowSpecificOrigins);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

        seedData(app.Services);
        app.Run();
    }
    private static void seedData(IServiceProvider services)
    {
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
                        Name = "Mass",
                    };
                    UnitCategory unitCategory2 = new UnitCategory()
                    {
                        Name = "Currency"
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
                    db.Users.Add(user);
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

                            }
                        },


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