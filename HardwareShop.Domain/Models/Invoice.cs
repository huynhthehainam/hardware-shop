using HardwareShop.Core.Bases;
using HardwareShop.Core.Helpers;
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public sealed class Invoice : EntityBase, ITrackingDate
    {
        public Invoice()
        {
        }

        public Invoice(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }

        public string Code { get; set; } = RandomStringHelper.RandomString(6);
        public int Id { get; set; }
        public int CustomerId { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader?.Load(this, ref customer);
            set => customer = value;
        }
        public string CustomerInformation { get; set; } = string.Empty;
        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader?.Load(this, ref shop);
            set => shop = value;
        }
        private ICollection<InvoiceDetail>? details;
        public ICollection<InvoiceDetail>? Details
        {
            get => lazyLoader?.Load(this, ref details);
            set => details = value;
        }

        public int? OrderId { get; set; }
        private Order? order;
        public Order? Order
        {
            get => lazyLoader?.Load(this, ref order);
            set => order = value;
        }
        public double Deposit { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

        public int? CurrentDebtHistoryId { get; set; }
        private CustomerDebtHistory? currentDebtHistory;
        public CustomerDebtHistory? CurrentDebtHistory
        {
            get => lazyLoader?.Load(this, ref currentDebtHistory);
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
