

using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public class AssetConfiguration : ModelConfigurationBase<Asset>
    {
        public AssetConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
            {
                _ = e.HasKey(e => e.Id);
            };
        }
    }
}