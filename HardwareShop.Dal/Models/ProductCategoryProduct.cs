﻿using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategoryProduct>(e =>
            {
                e.HasKey(e => new { e.ProductId, e.ProductCategoryId });
                e.HasOne(e => e.Product).WithMany(e => e.ProductCategoryProducts).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(e => e.ProductCategory).WithMany(e => e.ProductCategoryProducts).HasForeignKey(e => e.ProductCategoryId).OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}