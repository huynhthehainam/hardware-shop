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
    public class WarehouseProduct : EntityBase
    {
        public WarehouseProduct()
        {
        }

        public WarehouseProduct(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int ProductId { get; set; }
        private Product? product;
        public Product? Product
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref product) : product;
            set => product = value;
        }

        public int WarehouseId { get; set; }
        private Warehouse? warehouse;
        public Warehouse? Warehouse
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref warehouse) : warehouse;
            set => warehouse = value;
        }
        public double Quantity { get; set; }

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WarehouseProduct>(e =>
            {
                e.HasKey(e => new { e.ProductId, e.WarehouseId });
                e.HasOne(e => e.Product).WithMany(e => e.WarehouseProducts).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(e => e.Warehouse).WithMany(e => e.WarehouseProducts).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
