using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HardwareShop.Core.Models
{
    public class CacheUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public SystemUserRole Role { get; set; } = SystemUserRole.Staff;
        public string? Email { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = String.Empty;
    }
}
