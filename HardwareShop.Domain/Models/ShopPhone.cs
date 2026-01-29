using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public class ShopPhone : EntityBase
    {
        public ShopPhone()
        {
        }

        public ShopPhone(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }

        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader?.Load(this, ref shop);
            set => shop = value;
        }
        public Guid CountryId { get; set; }
        private Country? country;
        public Country? Country
        {
            get => lazyLoader?.Load(this, ref country);
            set => country = value;
        }
        public string OwnerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}