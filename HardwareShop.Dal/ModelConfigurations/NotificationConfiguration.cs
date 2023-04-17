

using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public class NotificationConfiguration : ModelConfigurationBase<Notification>
    {
        public NotificationConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
                {
                    _ = e.HasKey(e => e.Id);
                    _ = e.HasOne(e => e.User).WithMany(e => e.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                };
        }

    }
}