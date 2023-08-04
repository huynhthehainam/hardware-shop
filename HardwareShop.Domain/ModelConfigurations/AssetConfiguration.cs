using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Domain.ModelConfigurations
{
    public class AssetConfiguration : IEntityTypeConfiguration<Asset>
    {

        public void Configure(EntityTypeBuilder<Asset> e)
        {
            e.HasKey(e => e.Id);
        }
    }
}