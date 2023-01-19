using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
    public sealed class CustomerDeptHistory : EntityBase
    {
        public CustomerDeptHistory()
        {
        }

        public CustomerDeptHistory(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public int Id { get; set; }
        public double AmountOfChange { get; set; }
        public string? Reason { get; set; }

        public int CustomerDeptId { get; set; }
        private CustomerDept? customerDept;
        public CustomerDept? CustomerDept
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref customerDept) : customerDept;
            set => customerDept = value;
        }
    }
}
