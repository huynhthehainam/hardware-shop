using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
    public sealed class Shop : EntityBase
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

        private ShopAsset? shopAsset;
        public ShopAsset? ShopAsset
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shopAsset) : shopAsset;
            set => shopAsset = value;
        }

        private ICollection<AccountShop>? shopAccounts;
        public ICollection<AccountShop>? ShopAccounts
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shopAccounts) : shopAccounts;
            set => shopAccounts = value;
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

        private ICollection<Product>? products;
        public ICollection<Product>? Products
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref products) : products;
            set => products = value;
        }

        private ICollection<Invoice>? invoices;
        public ICollection<Invoice>? Invoices
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref invoices) : invoices;
            set => invoices = value;
        }
    }
}
