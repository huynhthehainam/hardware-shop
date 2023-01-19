using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
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
    }
}
