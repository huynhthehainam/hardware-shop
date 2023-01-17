using HardwareShop.Core.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Models
{
    public class PageData<T, T1> where T : EntityBase<T1> where T1 : struct
    {
        public List<T>? Items { get; set; }
        public int TotalRecords { get; set; } = 0;

    }
}
