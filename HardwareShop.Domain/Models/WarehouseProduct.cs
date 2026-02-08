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

        public Guid WarehouseId { get; set; }
        private Warehouse? warehouse;
        public Warehouse? Warehouse
        {
            get => lazyLoader?.Load(this, ref warehouse);
            set => warehouse = value;
        }
        public double Quantity { get; set; }
        public Guid ProductUnitId { get; set; }
        private ProductUnit? productUnit;
        public ProductUnit? ProductUnit
        {
            get => lazyLoader?.Load(this, ref productUnit);
            set => productUnit = value;
        }
    }
}
