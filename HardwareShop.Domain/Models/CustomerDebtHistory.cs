using System.Text.Json;
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Models
{

    public sealed class CustomerDebtHistory : EntityBase, ITrackingDate
    {
        public CustomerDebtHistory()
        {
        }

        public CustomerDebtHistory(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }

        public int Id { get; set; }
        public double OldDebt { get; set; }
        public double ChangeOfDebt { get; set; }
        public double NewDebt { get; set; }
        public string? Reason { get; set; }
        public String? ReasonParams { get; set; }
        public int CustomerDebtId { get; set; }
        private CustomerDebt? customerDebt;
        public CustomerDebt? CustomerDebt
        {
            get => lazyLoader?.Load(this, ref customerDebt);
            set => customerDebt = value;
        }
        private ICollection<Invoice>? invoices;
        public ICollection<Invoice>? Invoices
        {
            get => lazyLoader?.Load(this, ref invoices);
            set => invoices = value;
        }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }


    }
}
