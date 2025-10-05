using HardwareShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace HardwareShop.KafkaConsumer;

public static class Program
{
    public static void Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        Host.CreateDefaultBuilder(args)
           .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                var redisHost = configuration["RedisSettings:Host"];
                var redisPort = configuration["RedisSettings:Port"];
                var redisConnection = $"{redisHost}:{redisPort},connectTimeout=10000,syncTimeout=10000";
                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = redisConnection;
                });
                var connectionString = configuration.GetConnectionString("AppConn");
                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("Database connection string 'AppConn' is missing.");
                services.AddDbContext<MainDatabaseContext>(options =>
                  options.UseSqlServer(connectionString));
                services.AddHostedService<Worker>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build()
            .Run();
    }
}