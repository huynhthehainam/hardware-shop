using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public class ProductCategoryProduct : EntityBase
    {
        public ProductCategoryProduct()
        {
        }

        public ProductCategoryProduct(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public int ProductCategoryId { get; set; }
        private ProductCategory? productCategory;
        public ProductCategory? ProductCategory
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref productCategory) : productCategory;
            set => productCategory = value;
        }

        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref product) : product;
            set => product = value;
        }
    }
}
