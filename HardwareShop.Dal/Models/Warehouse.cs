using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public sealed class Warehouse : EntityBase
    {
        public Warehouse()
        {
        }

        public Warehouse(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string? Address { get; set; }

        public int ShopId { get; set; }

        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }
        private ICollection<WarehouseProduct>? warehouseProducts;
        public ICollection<WarehouseProduct>? WarehouseProducts
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref warehouseProducts) : warehouseProducts;
            set => warehouseProducts = value;
        }
    }
}
