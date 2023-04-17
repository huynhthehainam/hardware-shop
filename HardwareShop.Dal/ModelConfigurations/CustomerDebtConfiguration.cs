

using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class CustomerDebtConfiguration : ModelConfigurationBase<CustomerDebt>
    {
        public CustomerDebtConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
           {
               e.HasKey(e => e.CustomerId);
               e.HasOne(e => e.Customer).WithOne(e => e.Debt).HasForeignKey<CustomerDebt>(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
           };
        }

    }
}