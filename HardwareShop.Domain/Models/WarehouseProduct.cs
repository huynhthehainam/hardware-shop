using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public class WarehouseProduct : EntityBase
    {
        public WarehouseProduct()
        {
        }

        public WarehouseProduct(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader?.Load(this, ref product);
            set => product = value;
        }

        public int WarehouseId { get; set; }
        private Warehouse? warehouse;
        public Warehouse? Warehouse
        {
            get => lazyLoader?.Load(this, ref warehouse);
            set => warehouse = value;
        }
        public double Quantity { get; set; }
    }
}
