using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class UnitConfiguration : ModelConfigurationBase<Unit>
    {
        public UnitConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = u =>
            {
                _ = u.HasKey(entity => entity.Id);
                _ = u.HasOne(e => e.UnitCategory).WithMany(e => e.Units).HasForeignKey(e => e.UnitCategoryId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}