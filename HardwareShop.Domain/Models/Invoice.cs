using HardwareShop.Core.Bases;
using HardwareShop.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Domain.Models
{
    public sealed class Invoice : EntityBase, ITrackingDate
    {
        public Invoice()
        {
        }

        public Invoice(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public string Code { get; set; } = RandomStringHelper.RandomString(6);
        public int Id { get; set; }
        public int CustomerId { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref customer) : customer;
            set => customer = value;
        }
        public string CustomerInformation { get; set; } = string.Empty;

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
        // Calculate total cost

        public double GetTotalCost()
        {
            if (Details == null || Details.Count == 0)
            {
                return 0;
            }


            double cost = 0.0;
            foreach (InvoiceDetail detail in Details)
            {
                cost += detail.GetTotalCost();
            }
            Unit? cashUnit = Shop?.CashUnit;
            return cashUnit == null ? 0 : cashUnit.RoundValue(cost);
        }
    }
}
