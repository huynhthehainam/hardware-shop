using HardwareShop.Core.Bases;
using HardwareShop.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public sealed class Invoice : EntityBase, ITrackingDate
    {
        public Invoice()
        {
        }

        public Invoice(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public string Code { get; set; } = RandomStringHelper.RandomString(24);
        public int Id { get; set; }
        public int CustomerId { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref customer) : customer;
            set => customer = value;
        }

        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }
        private ICollection<InvoiceDetail>? details;
        public ICollection<InvoiceDetail>? Details
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref details) : details;
            set => details = value;
        }

        public int? OrderId { get; set; }
        private Order? order;
        public Order? Order
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref order) : order;
            set => order = value;
        }
        public double Deposit { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

        public int? CurrentDebtHistoryId { get; set; }
        private CustomerDebtHistory? currentDebtHistory;
        public CustomerDebtHistory? CurrentDebtHistory
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref currentDebtHistory) : currentDebtHistory;
            set => currentDebtHistory = value;
        }

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(e =>
                {
                    e.HasKey(e => e.Id);
                    e.HasOne(e => e.Customer).WithMany(e => e.Invoices).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(e => e.Shop).WithMany(e => e.Invoices).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(e => e.Order).WithMany(e => e.Invoices).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(e => e.CurrentDebtHistory).WithMany(e => e.Invoices).HasForeignKey(e => e.CurrentDebtHistoryId).OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
