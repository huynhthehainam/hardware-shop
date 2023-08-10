using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{

    public sealed class CustomerDebt : EntityBase
    {
        public CustomerDebt()
        {
        }

        public CustomerDebt(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public int CustomerId { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader?.Load(this, ref customer);
            set => customer = value;
        }
        public double Amount { get; set; }
        private ICollection<CustomerDebtHistory>? histories;
        public ICollection<CustomerDebtHistory>? Histories
        {
            get => lazyLoader?.Load(this, ref histories);
            set => histories = value;
        }


    }
}
