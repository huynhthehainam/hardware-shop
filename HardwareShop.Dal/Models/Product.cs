using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
    public sealed class Product : EntityBase
    {
        public Product()
        {
        }

        public Product(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public double? Mass { get; set; }
        public double? PricePerMass { get; set; }
        public double? PercentForFamiliarCustomer { get; set; }
        public double? PercentForCustomer { get; set; }
        public double? PriceForFamiliarCustomer { get; set; }
        public double PriceForCustomer { get; set; }
        public int UnitId { get; set; }
        private Unit? unit;
        public Unit? Unit
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref unit) : unit;
            set => unit = value;
        }

        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }

    }
}
