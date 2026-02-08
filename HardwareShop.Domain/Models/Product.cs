using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Models
{
    public sealed class Product : EntityBase, ISoftDeletable
    {
        public Product()
        {
        }

        public Product(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Name { get; set; } = string.Empty;
        public Guid ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader?.Load(this, ref shop);
            set => shop = value;
        }

        private ICollection<ProductUnit>? productUnits;
        public ICollection<ProductUnit>? ProductUnits
        {
            get => lazyLoader?.Load(this, ref productUnits);
            set => productUnits = value;
        }


        public bool IsDeleted { get; set; }

        private ICollection<ProductAsset>? productAssets;
        public ICollection<ProductAsset>? ProductAssets
        {
            get => lazyLoader?.Load(this, ref productAssets);
            set => productAssets = value;
        }
        private ICollection<WarehouseProduct>? warehouseProducts;
        public ICollection<WarehouseProduct>? WarehouseProducts
        {
            get => lazyLoader?.Load(this, ref warehouseProducts);
            set => warehouseProducts = value;
        }
        private ICollection<ProductCategoryProduct>? productCategoryProducts;
        public ICollection<ProductCategoryProduct>? ProductCategoryProducts
        {
            get => lazyLoader?.Load(this, ref productCategoryProducts);
            set => productCategoryProducts = value;
        }
        public double InventoryNumber => WarehouseProducts == null ? 0 : WarehouseProducts.Sum(e => e.Quantity);
    }
}
