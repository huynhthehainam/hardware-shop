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
    public sealed class Shop : EntityBase, ISoftDeletable, ITrackingDate
    {
        public Shop()
        {
        }

        public Shop(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }

        private ICollection<ShopAsset>? assets;
        public ICollection<ShopAsset>? Assets
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref assets) : assets;
            set => assets = value;
        }

        private ICollection<UserShop>? userShops;
        public ICollection<UserShop>? UserShops
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref userShops) : userShops;
            set => userShops = value;
        }

        private ICollection<ProductCategory>? productCategories;
        public ICollection<ProductCategory>? ProductCategories
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref productCategories) : productCategories;
            set => productCategories = value;
        }

        private ICollection<Warehouse>? warehouses;
        public ICollection<Warehouse>? Warehouses
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref warehouses) : warehouses;
            set => warehouses = value;
        }

        private ICollection<Invoice>? invoices;
        public ICollection<Invoice>? Invoices
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref invoices) : invoices;
            set => invoices = value;
        }
        private ICollection<Product>? products;
        public ICollection<Product>? Products
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref products) : products;
            set => products = value;
        }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shop>(s =>
            {
                s.HasKey(x => x.Id);
            });
        }


    }
}
