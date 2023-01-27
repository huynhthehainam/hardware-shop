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

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>(e =>
            {
                e.HasKey(e => e.Id);
                e.HasOne(e => e.Shop).WithMany(e => e.ProductCategories).HasForeignKey(e => e.ShopId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
