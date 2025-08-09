using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Application.Models
{
    public class PagingModel
    {
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
    }
}
