using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int UnitCategoryId { get; set; }
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
    }
}
