﻿using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
    public sealed class Invoice : EntityBase
    {
        public Invoice()
        {
        }

        public Invoice(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref customer) : customer;
            set => customer = value;
        }

        public int ShopId { get; set; }
        private Shop? shop;
        public Shop? Shop
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shop) : shop;
            set => shop = value;
        }

    }
}
