using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class UnitCategoryConfiguration : IEntityTypeConfiguration<UnitCategory>
    {
       

        public void Configure(EntityTypeBuilder<UnitCategory> uc)
        {
            _ = uc.HasKey(entity => entity.Id);

        }
    }
}