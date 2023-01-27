using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HardwareShop.Core.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountRole
    {
        Admin,
        Staff,
    }
}
