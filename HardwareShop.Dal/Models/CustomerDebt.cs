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
    public sealed class CustomerDebt : EntityBase
    {
        public CustomerDebt()
        {
        }

        public CustomerDebt(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int CustomerId { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref customer) : customer;
            set => customer = value;
        }
        public double AmountOfDebt { get; set; }

        private ICollection<CustomerDebtHistory>? histories;
        public ICollection<CustomerDebtHistory>? Histories
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref histories) : histories;
            set => histories = value;
        }
        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerDebt>(e =>
            {
                e.HasKey(e => e.CustomerId);
                e.HasOne(e => e.Customer).WithOne(e => e.Debt).HasForeignKey<CustomerDebt>(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}