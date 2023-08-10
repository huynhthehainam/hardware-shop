using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Domain.Models
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
        public double CompareWithPrimaryUnit { get; set; } = 1;
        public bool IsPrimary { get; set; }
        public int UnitCategoryId { get; set; }
        public double RoundValue(double value)
        {
            value /= StepNumber;
            value = Math.Round(value);
            value *= StepNumber;
            return value;
        }
        public string ConvertValueToString(double value)
        {
            int roundNumber = 0;
            while (StepNumber < Math.Pow(10, -roundNumber))
            {
                roundNumber++;
            }
            return value.ToString($"N{roundNumber}");
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
    }
}
