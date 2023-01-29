using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
    public sealed class Product : EntityBase, ISoftDeletable
    {
        public Product()
        {
        }

        public Product(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public double? Mass { get; set; }
        public double? PricePerMass { get; set; }
        public double? PercentForFamiliarCustomer { get; set; }
        public double? PercentForCustomer { get; set; }
        public double? PriceForFamiliarCustomer { get; set; }
        public double PriceForCustomer { get; set; }
        public int UnitId { get; set; }
        private Unit? unit;
        public Unit? Unit
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref unit) : unit;
            set => unit = value;
        }

        public int ProductCategoryId { get; set; }
        private ProductCategory? productCategory;
        public ProductCategory? ProductCategory
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref productCategory) : productCategory;
            set => productCategory = value;
        }


        private ICollection<InvoiceDetail>? invoiceDetails;
        public ICollection<InvoiceDetail>? InvoiceDetails
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref invoiceDetails) : invoiceDetails;
            set => invoiceDetails = value;
        }
        public bool IsDeleted { get; set; }

        private ICollection<ProductAsset>? productAssets;
        public ICollection<ProductAsset>? ProductAssets
        {
            get => lazyLoader != null ? lazyLoader.Load(this, ref productAssets) : productAssets;
            set => productAssets = value;
        }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(e => e.Id);
                e.HasOne(e => e.Unit).WithMany(e => e.Products).HasForeignKey(e => e.UnitId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(e => e.productCategory).WithMany(e => e.Products).HasForeignKey(e => e.ProductCategoryId).OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
