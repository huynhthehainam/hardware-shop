using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public class ProductCategoryProduct : EntityBase
    {
        public ProductCategoryProduct()
        {
        }

        public ProductCategoryProduct(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }

        public int ProductCategoryId { get; set; }
        private ProductCategory? productCategory;
        public ProductCategory? ProductCategory
        {
            get => lazyLoader?.Load(this, ref productCategory);
            set => productCategory = value;
        }

        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader?.Load(this, ref product);
            set => product = value;
        }
    }
}
