using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Domain.ModelConfigurations
{
    public sealed class UnitCategoryConfiguration : IEntityTypeConfiguration<UnitCategory>
    {
       

        public void Configure(EntityTypeBuilder<UnitCategory> uc)
        {
            _ = uc.HasKey(entity => entity.Id);

        }
    }
}