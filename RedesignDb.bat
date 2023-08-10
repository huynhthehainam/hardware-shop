
dotnet ef database drop -f --project HardwareShop.Infrastructure --startup-project HardwareShop.WebApi 
dotnet ef migrations remove --project HardwareShop.Infrastructure --startup-project HardwareShop.WebApi 
dotnet ef migrations add Initial --project HardwareShop.Infrastructure --startup-project HardwareShop.WebApi --output-dir Data\Migrations
dotnet ef database update --project HardwareShop.Infrastructure --startup-project HardwareShop.WebApi 
