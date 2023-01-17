using HardwareShop.Business.Implementations;
using HardwareShop.Business.Services;
using HardwareShop.Core.Implementations;
using HardwareShop.Core.Services;
using HardwareShop.Dal;
using HardwareShop.Dal.Extensions;
using HardwareShop.WebApi.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEntityFrameworkNpgsql().AddDbContext<MainDatabaseContext>((sp, opt) => opt.UseNpgsql(builder.Configuration.GetConnectionString("AppConn"), b =>
        {
            b.MigrationsAssembly("HardwareShop.WebApi");
        }).UseInternalServiceProvider(sp));
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

        #endregion

        builder.Services.AddSingleton<IResponseResultFactory, ResponseResultFactory>();
        builder.Services.AddSingleton<IHashsingPasswordService, HashingPasswordService>();
        builder.Services.ConfigureRepository();
        builder.Services.AddScoped<IAccountService, AccountService>();



        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");


        app.Run();
    }
}