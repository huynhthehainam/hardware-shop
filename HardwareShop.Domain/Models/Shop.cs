using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Enums;
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
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string? Name { get; set; }
        public string? Address { get; set; }
        public Language Language { get; set; } = Language.Vietnamese;

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


        public static Shop CreateShop(string name, int cashUnitId, string address, Language language = Language.Vietnamese)
        {

            var shop = new Shop
            {
                Name = name,
                CashUnitId = cashUnitId,
                Address = address,
                Language = language
            };
            shop.AddDomainEvent(new Events.ShopCreatedEvent()
            {
                Name = name,
                ShopId = shop.Id,
                Language = language
            });
            return shop;
        }

    }
}
