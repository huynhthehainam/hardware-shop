using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Models
{
    public sealed class Shop : EntityBase, ISoftDeletable, ITrackingDate
    {
        public Shop()
        {
        }

        public Shop(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string[]? Emails { get; set; }

        private ICollection<ShopAsset>? assets;
        public ICollection<ShopAsset>? Assets
        {
            get => lazyLoader?.Load(this, ref assets);
            set => assets = value;
        }

        private ICollection<UserShop>? userShops;
        public ICollection<UserShop>? UserShops
        {
            get => lazyLoader?.Load(this, ref userShops);
            set => userShops = value;
        }

        private ICollection<ProductCategory>? productCategories;
        public ICollection<ProductCategory>? ProductCategories
        {
            get => lazyLoader?.Load(this, ref productCategories);
            set => productCategories = value;
        }

        private ICollection<Warehouse>? warehouses;
        public ICollection<Warehouse>? Warehouses
        {
            get => lazyLoader?.Load(this, ref warehouses);
            set => warehouses = value;
        }

        private ICollection<Invoice>? invoices;
        public ICollection<Invoice>? Invoices
        {
            get => lazyLoader?.Load(this, ref invoices);
            set => invoices = value;
        }
        private ICollection<Product>? products;
        public ICollection<Product>? Products
        {
            get => lazyLoader?.Load(this, ref products);
            set => products = value;
        }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        private ICollection<Order>? orders;
        public ICollection<Order>? Orders
        {
            get => lazyLoader?.Load(this, ref orders);
            set => orders = value;
        }
        public int CashUnitId { get; set; }
        private Unit? cashUnit;
        public Unit? CashUnit
        {
            get => lazyLoader?.Load(this, ref cashUnit);
            set => cashUnit = value;
        }

        private ICollection<Customer>? customers;
        public ICollection<Customer>? Customers
        {
            get => lazyLoader?.Load(this, ref customers);
            set => customers = value;
        }
        private ICollection<ShopPhone>? phones;
        public ICollection<ShopPhone>? Phones
        {
            get => lazyLoader?.Load(this, ref phones);
            set => phones = value;
        }
        private ShopSetting? shopSetting;
        public ShopSetting? ShopSetting
        {
            get => lazyLoader?.Load(this, ref shopSetting);
            set => shopSetting = value;
        }


    }
}
