using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
    public sealed class CustomerDept : EntityBase
    {
        public CustomerDept()
        {
        }

        public CustomerDept(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int CustomerId { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref customer) : customer;
            set => customer = value;
        }
        public double AmountOfDept { get; set; }

        private ICollection<CustomerDeptHistory>? histories;
        public ICollection<CustomerDeptHistory>? Histories
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref histories) : histories;
            set => histories = value;
        }
    }
}
