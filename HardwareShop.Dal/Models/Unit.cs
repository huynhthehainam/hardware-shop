using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public sealed class Unit : EntityBase
    {
        public Unit()
        {
        }

        public Unit(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double StepNumber { get; set; } = 0.01;
        public int UnitCategoryId { get; set; }
        public double RoundValue(double value)
        {
            value = value / StepNumber;
            value = Math.Round(value);
            value = value * StepNumber;
            return value;
        }
        private UnitCategory? unitCategory;
        public UnitCategory? UnitCategory
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref unitCategory) : unitCategory;
            set => unitCategory = value;
        }

        private ICollection<Product>? products;
        public ICollection<Product>? Products
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref products) : products;
            set => products = value;
        }
        private ICollection<Shop>? shops;
        public ICollection<Shop>? Shops
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shops) : shops;
            set => shops = value;
        }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Unit>(entity =>
            {
                entity.HasKey(entity => entity.Id);
                entity.HasOne(e => e.UnitCategory).WithMany(e => e.Units).HasForeignKey(e => e.UnitCategoryId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
