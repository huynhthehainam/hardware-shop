using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Dal.ModelConfigurations
{
    public class AssetConfiguration : IEntityTypeConfiguration<Asset>
    {

        public void Configure(EntityTypeBuilder<Asset> e)
        {
            e.HasKey(e => e.Id);
        }
    }
}