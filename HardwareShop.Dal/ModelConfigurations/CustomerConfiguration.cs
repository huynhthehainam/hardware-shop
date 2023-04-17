

using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class CustomerConfiguration : ModelConfigurationBase<Customer>
    {
        public CustomerConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
            {
                e.HasKey(e => e.Id);
                e.HasOne(e => e.Shop).WithMany(e => e.Customers).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(e => e.PhoneCountry).WithMany(e => e.Customers).HasForeignKey(e => e.PhoneCountryId).OnDelete(DeleteBehavior.SetNull);
            };
        }
    }
}