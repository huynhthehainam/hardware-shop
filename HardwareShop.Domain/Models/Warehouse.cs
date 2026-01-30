using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public sealed class Warehouse : EntityBase
    {
        public Warehouse()
        {
        }

        public Warehouse(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }

        public Guid ShopId { get; set; }

        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader?.Load(this, ref shop);
            set => shop = value;
        }
        private ICollection<WarehouseProduct>? warehouseProducts;
        public ICollection<WarehouseProduct>? WarehouseProducts
        {
            get => lazyLoader?.Load(this, ref warehouseProducts);
            set => warehouseProducts = value;
        }

        public static Warehouse Create(string name, string? address, Guid shopId)
        {
            return new Warehouse
            {
                Id = Guid.CreateVersion7(),
                Name = name,
                Address = address,
                ShopId = shopId
            };
        }
    }
}
