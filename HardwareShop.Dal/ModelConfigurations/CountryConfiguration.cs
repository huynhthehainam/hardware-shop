using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Dal.ModelConfigurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {

        public void Configure(EntityTypeBuilder<Country> e)
        {
            _ = e.HasKey(e => e.Id);
        }
    }
}