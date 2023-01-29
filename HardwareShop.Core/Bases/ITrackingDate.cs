using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Bases
{
    public interface ITrackingDate
    {
        DateTime CreatedDate { get; set; } 
        DateTime? LastModifiedDate { get; set; }
    }
}
