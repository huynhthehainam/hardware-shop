using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public sealed class Customer : EntityBase
    {
        public Customer()
        {
        }

        public Customer(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public int? PhoneCountryId { get; set; }
        private Country? phoneCountry;
        public Country? PhoneCountry
        {
            get => lazyLoader.Load(this, ref phoneCountry);
            set => phoneCountry = value;
        }
        public string? Address { get; set; }
        public int ShopId { get; set; }
        public bool IsFamiliar { get; set; } = false;
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }

        private CustomerDebt? debt;
        public CustomerDebt? Debt
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref debt) : debt;
            set => debt = value;
        }
        private ICollection<Invoice>? invoices;
        public ICollection<Invoice>? Invoices
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref invoices) : invoices;
            set => invoices = value;
        }
        private ICollection<Order>? orders;
        public ICollection<Order>? Orders
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref orders) : orders;
            set => orders = value;
        }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(e => e.Id);
                e.HasOne(e => e.Shop).WithMany(e => e.Customers).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(e => e.PhoneCountry).WithMany(e => e.Customers).HasForeignKey(e => e.PhoneCountryId).OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
