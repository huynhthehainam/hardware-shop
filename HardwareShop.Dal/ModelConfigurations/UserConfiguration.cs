using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class UserConfiguration : ModelConfigurationBase<User>
    {
        public UserConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = u =>
            {
                _ = u.HasKey(a => a.Id);
                _ = u.HasIndex(e => e.Username).IsUnique();
                _ = u.HasOne(e => e.PhoneCountry).WithMany(e => e.Users).HasForeignKey(e => e.PhoneCountryId).OnDelete(DeleteBehavior.SetNull);
            };
        }
    }
}