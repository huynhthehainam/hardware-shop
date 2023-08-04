using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Domain.ModelConfigurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {


        public void Configure(EntityTypeBuilder<Notification> e)
        {
            _ = e.HasKey(e => e.Id);
            _ = e.HasOne(e => e.User).WithMany(e => e.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}