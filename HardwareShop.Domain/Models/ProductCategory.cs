using HardwareShop.Core.Bases;
using HardwareShop.Domain.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Domain.Models
{
    public sealed class ProductCategory : EntityBase, ISoftDeletable
    {
        public ProductCategory()
        {
        }

        public ProductCategory(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string? Description { get; set; }
        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }
        public bool IsDeleted { get; set; }
        private ICollection<ProductCategoryProduct>? productCategoryProducts;
        public ICollection<ProductCategoryProduct>? ProductCategoryProducts
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref productCategoryProducts) : productCategoryProducts;
            set => productCategoryProducts = value;
        }
    }
}
