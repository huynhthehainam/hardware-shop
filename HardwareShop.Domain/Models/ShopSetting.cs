



using HardwareShop.Domain.Abstracts;
using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Domain.Models
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
    }
}