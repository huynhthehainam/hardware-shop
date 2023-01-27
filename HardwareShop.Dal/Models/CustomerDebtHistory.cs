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
    public sealed class CustomerDebtHistory : EntityBase
    {
        public CustomerDebtHistory()
        {
        }

        public CustomerDebtHistory(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public int Id { get; set; }
        public double AmountOfChange { get; set; }
        public string? Reason { get; set; }

        public int CustomerDebtId { get; set; }
        private CustomerDebt? customerDebt;
        public CustomerDebt? CustomerDebt
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref customerDebt) : customerDebt;
            set => customerDebt = value;
        }

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerDebtHistory>(h =>
            {
                h.HasKey(e => e.Id);
                h.HasOne(e => e.CustomerDebt).WithMany(e => e.Histories).HasForeignKey(e => e.CustomerDebtId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
