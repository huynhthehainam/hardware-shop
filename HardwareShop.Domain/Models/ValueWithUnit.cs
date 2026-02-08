using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public class ValueWithUnit : EntityBase
    {
        public ValueWithUnit()
        {
        }

        public ValueWithUnit(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }

        public Guid Id { get; set; } = Guid.CreateVersion7();
        public decimal Value { get; set; }
        public int UnitId { get; set; }
        private Unit? unit;
        public Unit? Unit
        {
            get => lazyLoader?.Load(this, ref unit);
            set => unit = value;
        }

    }
}