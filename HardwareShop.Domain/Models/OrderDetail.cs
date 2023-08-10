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
        public long Id { get; set; }
        public int OrderId { get; set; }
        private Order? order;
        public Order? Order
        {
            get => lazyLoader?.Load(this, ref order);
            set => order = value;
        }

        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader?.Load(this, ref product);
            set => product = value;
        }

        public double Quantity { get; set; }
        public string? Description { get; set; }

    }
}
