using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public sealed class OrderDetail : EntityBase
    {
        public OrderDetail()
        {
        }

        public OrderDetail(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid OrderId { get; set; }
        private Order? order;
        public Order? Order
        {
            get => lazyLoader?.Load(this, ref order);
            set => order = value;
        }

        public double Quantity { get; set; }
        public string? Note { get; set; }
        public double UnitPrice { get; set; }
        public Guid ProductUnitId { get; set; }
        private ProductUnit? productUnit;
        public ProductUnit? ProductUnit
        {
            get => lazyLoader?.Load(this, ref productUnit);
            set => productUnit = value;
        }
    }
}
