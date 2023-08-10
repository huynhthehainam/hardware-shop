using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
       

        public void Configure(EntityTypeBuilder<User> u)
        {
            _ = u.HasKey(a => a.Id);
            _ = u.HasIndex(e => e.Username).IsUnique();
            _ = u.HasOne(e => e.PhoneCountry).WithMany(e => e.Users).HasForeignKey(e => e.PhoneCountryId).OnDelete(DeleteBehavior.SetNull);
            _ = u.HasIndex(e => e.Guid);

        }
    }
}