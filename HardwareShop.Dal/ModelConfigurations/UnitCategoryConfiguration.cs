using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class UnitCategoryConfiguration : ModelConfigurationBase<UnitCategory>
    {
        public UnitCategoryConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = uc =>
            {
                _ = uc.HasKey(entity => entity.Id);
            };
        }
    }
}