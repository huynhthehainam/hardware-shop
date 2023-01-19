using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
    public sealed class ShopAsset : EntityBase
    {
        public ShopAsset()
        {
        }

        public ShopAsset(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }
        public byte[]? Logo { get; set; }

    }
}
