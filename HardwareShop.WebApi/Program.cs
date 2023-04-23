
using System.Text;
using HardwareShop.Business.Extensions;
using HardwareShop.Core.Implementations;
using HardwareShop.Core.Services;
using HardwareShop.Dal;
using HardwareShop.Dal.Extensions;
using HardwareShop.WebApi.Configurations;
using HardwareShop.WebApi.Extensions;
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
        var customCorsUrls = new string[] { "http://localhost:3000", "https://cuahangsatthep.shop" };
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: mainAllowSpecificOrigins, builder =>
            {
                builder.WithOrigins(
                customCorsUrls
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
                Array.Empty<string>()
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
               x.Events = new JwtBearerEvents
               {
                   OnMessageReceived = (context) =>
               {
                   var bb = context.HttpContext.Request.Query["access_token"];
                   context.Token = context.HttpContext.Request.Query["access_token"];
                   return Task.CompletedTask;
               }
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
                       return notBefore.HasValue ? notBefore.Value <= DateTime.UtcNow : false ||
!expires.HasValue || expires.Value >= DateTime.UtcNow;
                   }
               };
           });

        #region Configuration
        builder.Services.Configure<HashingConfiguration>(builder.Configuration.GetSection("HashingConfiguration"));
        builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfiguration"));

        #endregion
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddScoped<ILanguageService, LanguageService>();
        builder.Services.AddScoped<IResponseResultBuilder, ResponseResultBuilder>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddSingleton<IHashingPasswordService, HashingPasswordService>();
        builder.Services.ConfigureRepository();
        builder.Services.ConfigureBusiness();


        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseCors(mainAllowSpecificOrigins);


        app.MapControllers();
        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        app.UseAuthentication();

        app.UseAuthorization();

        app.SeedData();
        app.Run();
    }


}