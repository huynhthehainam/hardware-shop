using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Models
{
    public class CacheAccount
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public AccountRole Role { get; set; } = AccountRole.Staff;

    }
}
