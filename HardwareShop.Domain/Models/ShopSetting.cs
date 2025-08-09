



using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public class ShopSetting : EntityBase
    {
        public ShopSetting()
        {
        }

        public ShopSetting(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader?.Load(this, ref shop);
            set => shop = value;
        }
        public bool IsAllowedToShowInvoiceDownloadOptions { get; set; } = true;
    }
}