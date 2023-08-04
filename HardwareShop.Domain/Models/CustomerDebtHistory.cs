using System.Text.Json;
using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Domain.Models
{

    public sealed class CustomerDebtHistory : EntityBase, ITrackingDate
    {
        public CustomerDebtHistory()
        {
        }

        public CustomerDebtHistory(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public int Id { get; set; }
        public double OldDebt { get; set; }
        public double ChangeOfDebt { get; set; }
        public double NewDebt { get; set; }
        public string? Reason { get; set; }
        public JsonDocument? ReasonParams { get; set; }
        public int CustomerDebtId { get; set; }
        private CustomerDebt? customerDebt;
        public CustomerDebt? CustomerDebt
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref customerDebt) : customerDebt;
            set => customerDebt = value;
        }
        private ICollection<Invoice>? invoices;
        public ICollection<Invoice>? Invoices
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref invoices) : invoices;
            set => invoices = value;
        }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }


    }
}
