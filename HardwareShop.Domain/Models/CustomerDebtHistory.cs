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

        public Guid Id { get; set; } = Guid.CreateVersion7();
        public double OldDebt { get; set; }
        public double ChangeOfDebt { get; set; }
        public double NewDebt { get; set; }
        public string? Reason { get; set; }
        public String? ReasonParams { get; set; }
        public Guid CustomerDebtId { get; set; }
        private CustomerDebt? customerDebt;
        public CustomerDebt? CustomerDebt
        {
            get => lazyLoader?.Load(this, ref customerDebt);
            set => customerDebt = value;
        }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }


    }
}
