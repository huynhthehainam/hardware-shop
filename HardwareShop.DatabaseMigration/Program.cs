using System.Net.Http.Json;
using System.Threading.Tasks;
using HardwareShop.Application.Services;
using HardwareShop.Infrastructure.Data;
using HardwareShop.Infrastructure.Services; // Add this using
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HardwareShop.DatabaseMigration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Determine primary connection string key: prefer "Primary", fallback to "AppConn"
                    var primaryConn = context.Configuration.GetConnectionString("Primary");
                    services.AddDbContext<MainDatabaseContext>(options =>
                        options.UseSqlServer(primaryConn));

                    services.AddScoped<ISeedingService, SeedingService>(); // Register SeedingService
                    services.Configure<HashingConfiguration>(context.Configuration.GetSection("HashingConfiguration")); // If needed for seeding
                    services.AddSingleton<IHashingPasswordService, HashingPasswordService>(); // If needed for seeding
                    services.AddHttpClient();
                })
                .Build();

            // Run migrations for all configured connection strings
            var config = host.Services.GetRequiredService<IConfiguration>();
            var connSection = config.GetSection("ConnectionStrings");
            foreach (var child in connSection.GetChildren())
            {
                var name = child.Key;
                var conn = child.Value;
                if (string.IsNullOrWhiteSpace(conn))
                    continue;

                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder<MainDatabaseContext>();
                    optionsBuilder.UseSqlServer(conn);
                    using (var ctx = new MainDatabaseContext(optionsBuilder.Options))
                    {
                        Console.WriteLine($"Applying migrations for connection '{name}'...");
                        ctx.Database.Migrate();
                        Console.WriteLine($"Migrations applied for '{name}'.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to migrate database for '{name}': {ex.Message}");
                }
            }

            // Run seeding only on primary database (registered in DI)
            using (var scope = host.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<ISeedingService>();
                bool isDevelopment = environment == "Development" || environment == "DevContainer";
                await seeder.SeedDataAsync(isDevelopment);
                Console.WriteLine("Database seeding completed.");
            }
        }
    }
}