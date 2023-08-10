using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {

        public void Configure(EntityTypeBuilder<Unit> u)
        {
            _ = u.HasKey(entity => entity.Id);
            _ = u.HasOne(e => e.UnitCategory).WithMany(e => e.Units).HasForeignKey(e => e.UnitCategoryId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}