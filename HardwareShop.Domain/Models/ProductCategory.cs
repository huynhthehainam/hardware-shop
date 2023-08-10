using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Models
{
    public sealed class ProductCategory : EntityBase, ISoftDeletable
    {
        public ProductCategory()
        {
        }

        public ProductCategory(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string? Description { get; set; }
        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader?.Load(this, ref shop);
            set => shop = value;
        }
        public bool IsDeleted { get; set; }
        private ICollection<ProductCategoryProduct>? productCategoryProducts;
        public ICollection<ProductCategoryProduct>? ProductCategoryProducts
        {
            get => lazyLoader?.Load(this, ref productCategoryProducts);
            set => productCategoryProducts = value;
        }
    }
}
