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
        public static void Main(string[] args)
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
                    var connectionString = context.Configuration.GetConnectionString("AppConn");
                    services.AddDbContext<MainDatabaseContext>(options =>
                        options.UseSqlServer(connectionString));
                    services.AddScoped<ISeedingService, SeedingService>(); // Register SeedingService
                    services.Configure<HashingConfiguration>(context.Configuration.GetSection("HashingConfiguration")); // If needed for seeding
                    services.AddSingleton<IHashingPasswordService, HashingPasswordService>(); // If needed for seeding
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MainDatabaseContext>();
                db.Database.Migrate();
                Console.WriteLine("Database migration completed.");

                // Run seeding
                var seeder = scope.ServiceProvider.GetRequiredService<ISeedingService>();
                bool isDevelopment = environment == "Development" || environment == "DevContainer";
                seeder.SeedData(isDevelopment);
                Console.WriteLine("Database seeding completed.");
            }
        }
    }
}