using System.Globalization;
using System.Text;
using HardwareShop.Application.Services;
using HardwareShop.Infrastructure.Extensions;
using HardwareShop.Infrastructure.Kafka;
using HardwareShop.Infrastructure.Outbox;
using HardwareShop.Infrastructure.Saga;
using HardwareShop.Infrastructure.Services;
using HardwareShop.WebApi.Configurations;
using HardwareShop.WebApi.Extensions;
using HardwareShop.WebApi.GraphQL;
using HardwareShop.WebApi.GrpcServices;
using HardwareShop.WebApi.Hubs;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace HardwareShop.WebApi;
// public class HasScopeRequirement : IAuthorizationRequirement
// {

//     public HasScopeRequirement()
//     {
//         var a = 0;

//     }
// }
// public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
// {
//     protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
//     {

//         context.Succeed(requirement);

//     }
// }

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var mainAllowSpecificOrigins = "mainAllowSpecificOrigins";
        var customCorsUrls = new string[] { "http://localhost:3000", "http://localhost:4200", "https://cuahangsatthep.shop" };
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: mainAllowSpecificOrigins, builder =>
            {
                builder.WithOrigins(
                    customCorsUrls
                ).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            });
        });
        var aa = builder.Configuration.GetConnectionString("AppConn");

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        });
        builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
            var cultures = new CultureInfo[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("vi-VN"),
            };
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
        });
        builder.Services.AddGraphQLServer().AddAuthorization().AddQueryType<Query>().AddMutationType<Mutation>();
        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();
        builder.Services.AddSignalR();

        #region OutboxDispatcher
        builder.Services.AddHostedService<OutboxDispatcher>();
        #endregion

        #region ServicesConsumers
        builder.Services.AddHostedService<FlightKafkaSagaConsumer>();
        builder.Services.AddHostedService<HotelKafkaSagaConsumer>();
        #endregion

        #region MediatR
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(Program).Assembly,                      // WebApi (optional)
                typeof(HardwareShop.Application.AssemblyMarker).Assembly,
                typeof(HardwareShop.Infrastructure.AssemblyMarker).Assembly
            ));
        #endregion
        #region KafkaSagaOrchestrator
        builder.Services.AddScoped<BookingSagaOrchestrator>();
        #endregion

        builder.Services.AddDistributedRedisCache(option =>
        {
            option.Configuration = builder.Configuration["RedisSettings:Host"] + ":" +
                                   builder.Configuration["RedisSettings:Port"] +
                                   ",connectTimeout=10000,syncTimeout=10000";
        });


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
 {
     // 1. Define the Security Scheme
     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
     {
         Name = "Authorization",
         Type = SecuritySchemeType.ApiKey,
         Scheme = "Bearer",
         BearerFormat = "JWT",
         In = ParameterLocation.Header,
         Description = "Enter 'Bearer {token}'"
     });

     // 2. Add the Security Requirement
     // Note: In v10/v2.x, some internal reference handling has changed, 
     // but the standard AddSecurityRequirement signature remains source-compatible 
     // for most common JWT setups.
     options.AddSecurityRequirement(doc =>
     {
         var openApiSecurityRequirement = new OpenApiSecurityRequirement
         {
             { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() }
         };
         return openApiSecurityRequirement;
     });
 });



        // Keycloak OpenID Connect configuration
        var keycloakInternalUrl = builder.Configuration["Keycloak:InternalUrl"] ?? ""; // internal
        var keycloakIssuerUrl = builder.Configuration["Keycloak:IssuerUrl"] ?? ""; // external
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = keycloakInternalUrl;
            options.MetadataAddress = $"{keycloakInternalUrl}/.well-known/openid-configuration";
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = keycloakIssuerUrl,
                ValidateLifetime = true,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.HttpContext.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(token) && path.StartsWithSegments(ChatHubConstants.Endpoint))
                    {
                        context.Request.Headers["Authorization"] = $"Bearer {token}";
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = ctx =>
                {
                    Console.WriteLine($"JWT error: {ctx.Exception.Message}");
                    return Task.CompletedTask;
                }
            };
        });


        #region Configuration

        builder.Services.Configure<HashingConfiguration>(builder.Configuration.GetSection("HashingConfiguration"));
        builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfiguration"));

        #endregion

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddScoped<IResponseResultBuilder, ResponseResultBuilder>();
        builder.Services.AddSingleton<IChatHubController, ChatHubController>();

        builder.Services.ConfigureInfrastructure(builder.Configuration);


        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseCors(mainAllowSpecificOrigins);


        app.MapControllers();
        app.UseRequestLocalization();
        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        app.MapGraphQL();

        app.UseAuthentication();

        app.UseAuthorization();
        app.MapGrpcService<UserGrpcService>();
        if (app.Environment.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }

        app.MapHub<ChatHub>(ChatHubConstants.Endpoint);
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            Console.WriteLine("🚀 WEB APP STARTED");
        });
        app.Run();
    }
}