using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Dal.Models
{
    public class Shop : EntityBase<int>
    {
        public Shop()
        {
        }

        public Shop(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public string? Name { get; set; }
        public string? Address { get; set; }
        public byte[]? Logo { get; set; }


        private ICollection<ShopAccount> shopAccounts;
        public ICollection<ShopAccount> ShopAccounts
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref shopAccounts) : shopAccounts;
            set => shopAccounts = value;
        }
    }
}
