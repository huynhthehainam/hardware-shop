using HardwareShop.Core.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Models
{
    public class PageData<T> where T : EntityBase
    {
        public List<T> Items { get; set; } = new List<T>(); 
        public int TotalRecords { get; set; } = 0;

    }
}
