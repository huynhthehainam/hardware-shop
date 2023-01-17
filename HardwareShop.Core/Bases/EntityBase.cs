using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Bases
{
    public class EntityBase<T> where T : struct
    {
        public T Id { get; set; } = default(T);
        protected ILazyLoader? lazyLoader;
        public EntityBase(ILazyLoader lazyLoader)
        {
            this.lazyLoader = lazyLoader;
        }
        public EntityBase() { }
    }
}
