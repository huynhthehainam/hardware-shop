using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public sealed class Shop : EntityBase, ISoftDeletable, ITrackingDate
    {
        public Shop()
        {
        }

        public Shop(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string[]? Emails { get; set; }

        private ICollection<ShopAsset>? assets;
        public ICollection<ShopAsset>? Assets
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref assets) : assets;
            set => assets = value;
        }

        private ICollection<UserShop>? userShops;
        public ICollection<UserShop>? UserShops
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref userShops) : userShops;
            set => userShops = value;
        }

        private ICollection<ProductCategory>? productCategories;
        public ICollection<ProductCategory>? ProductCategories
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref productCategories) : productCategories;
            set => productCategories = value;
        }

        private ICollection<Warehouse>? warehouses;
        public ICollection<Warehouse>? Warehouses
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref warehouses) : warehouses;
            set => warehouses = value;
        }

        private ICollection<Invoice>? invoices;
        public ICollection<Invoice>? Invoices
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref invoices) : invoices;
            set => invoices = value;
        }
        private ICollection<Product>? products;
        public ICollection<Product>? Products
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref products) : products;
            set => products = value;
        }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        private ICollection<Order>? orders;
        public ICollection<Order>? Orders
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref orders) : orders;
            set => orders = value;
        }
        public int CashUnitId { get; set; }
        private Unit? cashUnit;
        public Unit? CashUnit
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref cashUnit) : cashUnit;
            set => cashUnit = value;
        }

        private ICollection<Customer>? customers;
        public ICollection<Customer>? Customers
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref customers) : customers;
            set => customers = value;
        }
        private ICollection<ShopPhone>? phones;
        public ICollection<ShopPhone>? Phones
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref phones) : phones;
            set => phones = value;
        }
        private ShopSetting? shopSetting;
        public ShopSetting? ShopSetting
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shopSetting) : shopSetting;
            set => shopSetting = value;
        }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Shop>(s =>
            {
                _ = s.HasKey(x => x.Id);
                _ = s.HasOne(e => e.CashUnit).WithMany(e => e.Shops).HasForeignKey(e => e.CashUnitId).OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}
