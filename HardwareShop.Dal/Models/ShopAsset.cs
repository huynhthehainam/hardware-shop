using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
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
        public byte[]? Bytes { get; set; }
        public string Filename { get; set; } = string.Empty;


        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopAsset>(s =>
            {
                s.HasKey(s => s.ShopId);
                s.HasOne(e => e.Shop).WithOne(e => e.ShopAsset).HasForeignKey<ShopAsset>(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
