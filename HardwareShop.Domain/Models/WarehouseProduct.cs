using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Domain.Models
{
    public class WarehouseProduct : EntityBase
    {
        public WarehouseProduct()
        {
        }

        public WarehouseProduct(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref product) : product;
            set => product = value;
        }

        public int WarehouseId { get; set; }
        private Warehouse? warehouse;
        public Warehouse? Warehouse
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref warehouse) : warehouse;
            set => warehouse = value;
        }
        public double Quantity { get; set; }
    }
}
