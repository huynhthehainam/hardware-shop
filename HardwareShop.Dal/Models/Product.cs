using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
        public double OriginalPrice { get; set; }
        public double PriceForCustomer { get; set; }
        public bool HasAutoCalculatePermission { get; set; } = false;
        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }

        public int UnitId { get; set; }
        private Unit? unit;
        public Unit? Unit
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref unit) : unit;
            set => unit = value;
        }
        private ICollection<InvoiceDetail>? invoiceDetails;
        public ICollection<InvoiceDetail>? InvoiceDetails
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref invoiceDetails) : invoiceDetails;
            set => invoiceDetails = value;
        }
        private ICollection<OrderDetail>? orderDetails;
        public ICollection<OrderDetail>? OrderDetails
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref orderDetails) : orderDetails;
            set => orderDetails = value;
        }
        public bool IsDeleted { get; set; }

        private ICollection<ProductAsset>? productAssets;
        public ICollection<ProductAsset>? ProductAssets
        {
            get => lazyLoader != null ? lazyLoader.Load(this, ref productAssets) : productAssets;
            set => productAssets = value;
        }
        private ICollection<WarehouseProduct>? warehouseProducts;
        public ICollection<WarehouseProduct>? WarehouseProducts
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref warehouseProducts) : warehouseProducts;
            set => warehouseProducts = value;
        }
        private ICollection<ProductCategoryProduct>? productCategoryProducts;
        public ICollection<ProductCategoryProduct>? ProductCategoryProducts
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref productCategoryProducts) : productCategoryProducts;
            set => productCategoryProducts = value;
        }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(e => e.Id);
                e.HasOne(e => e.Unit).WithMany(e => e.Products).HasForeignKey(e => e.UnitId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(e => e.Shop).WithMany(e => e.Products).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
