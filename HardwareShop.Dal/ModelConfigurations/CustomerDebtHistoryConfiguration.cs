

using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public class CustomerDebtHistoryConfiguration : ModelConfigurationBase<CustomerDebtHistory>
    {
        public CustomerDebtHistoryConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = h =>
            {
                h.HasKey(e => e.Id);
                h.HasOne(e => e.CustomerDebt).WithMany(e => e.Histories).HasForeignKey(e => e.CustomerDebtId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}