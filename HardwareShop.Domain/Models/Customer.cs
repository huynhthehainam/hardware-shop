using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public sealed class Customer : EntityBase
    {
        public Customer()
        {
        }

        public Customer(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public int? PhoneCountryId { get; set; }
        private Country? phoneCountry;
        public Country? PhoneCountry
        {
            get => lazyLoader?.Load(this, ref phoneCountry);
            set => phoneCountry = value;
        }
        public string? Address { get; set; }
        public int ShopId { get; set; }
        public bool IsFamiliar { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get =>  lazyLoader?.Load(this, ref shop);
            set => shop = value;
        }

        private CustomerDebt? debt;
        public CustomerDebt? Debt
        {
            get =>  lazyLoader?.Load(this, ref debt);
            set => debt = value;
        }
        private ICollection<Invoice>? invoices;
        public ICollection<Invoice>? Invoices
        {
            get =>  lazyLoader?.Load(this, ref invoices);
            set => invoices = value;
        }
        private ICollection<Order>? orders;
        public ICollection<Order>? Orders
        {
            get =>  lazyLoader?.Load(this, ref orders);
            set => orders = value;
        }
    }
}
