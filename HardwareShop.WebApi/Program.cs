using HardwareShop.Business.Extensions;
using HardwareShop.Business.Implementations;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Implementations;
using HardwareShop.Core.Services;
using HardwareShop.Dal;
using HardwareShop.Dal.Extensions;
using HardwareShop.Dal.Models;
using HardwareShop.WebApi.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data.Common;
using System.Text;

namespace HardwareSop.WebApi;
public class Program
{
    private static void seedData(IServiceProvider services)
    {
        const string assetFolder = "SampleImages";
        const string productAssetFile = "ProductAsset.jpg";
        const string shopAssetFile = "ShopAsset.jpg";
        const string userAssetFile = "UserAsset.jpg";
        using (var scope = services.CreateScope())
        {
            IWebHostEnvironment env = scope.ServiceProvider.GetService<IWebHostEnvironment>();
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
                    db.UnitCategories.Add(unitCategory);
                    db.SaveChanges();

                    Unit unit = new Unit { Name = "Kg", UnitCategory = unitCategory };
                    db.Units.Add(unit);
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
                        ProductCategories = new ProductCategory[]
                        {
                            new ProductCategory
                            {
                                Name = "Hoa Sen",
                                Description = "Hoa Sen",
                                Products = new Product[]
                                {
                                    new Product
                                    {
                                        Name = "H13x26",
                                        Mass =  2.5,
                                        Unit = unit,
                                        PercentForCustomer = 8,
                                        PriceForCustomer = 12000,
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
                                    }
                                }
                            }
                        }

                    };
                    db.Shops.Add(shop);
                    db.SaveChanges();
                }
            }
        }
    }
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



        var authConfigurationSection = builder.Configuration.GetSection("AuthConfiguration");
        builder.Services.Configure<AuthConfiguration>(authConfigurationSection);
        var appSettings = authConfigurationSection.Get<AuthConfiguration>();
        var key = Encoding.ASCII.GetBytes(appSettings.AuthSecret ?? "");
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
}