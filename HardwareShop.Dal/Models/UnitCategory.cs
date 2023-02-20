using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public static class UnitCategoryConstants
    {
        public const string CurrencyCategoryName = "Currency";
        public const string MassCategoryName = "Mass";

    }
    public sealed class UnitCategory : EntityBase
    {
        public UnitCategory()
        {
        }

        public UnitCategory(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        private ICollection<Unit>? units;
        public ICollection<Unit>? Units
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref units) : units;
            set => units = value;
        }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitCategory>(entity =>
            {
                entity.HasKey(entity => entity.Id);
            });
        }
    }
}
