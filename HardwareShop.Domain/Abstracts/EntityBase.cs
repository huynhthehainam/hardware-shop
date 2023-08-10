using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Domain.Abstracts
{
    public abstract class EntityBase
    {
        protected ILazyLoader? lazyLoader;
        public EntityBase(ILazyLoader lazyLoader)
        {
            this.lazyLoader = lazyLoader;
        }
        public EntityBase() { }
    }
}
