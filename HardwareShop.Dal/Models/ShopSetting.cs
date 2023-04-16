



using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public class ShopSetting : EntityBase
    {
        public ShopSetting()
        {
        }

        public ShopSetting(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }
        public bool IsAllowedToShowInvoiceDownloadOptions { get; set; } = true;
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ShopSetting>(s =>
            {
                _ = s.HasKey(s => s.ShopId);
                _ = s.HasOne(e => e.Shop).WithOne(e => e.ShopSetting).HasForeignKey<ShopSetting>(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}