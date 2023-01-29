using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HardwareShop.Core.Constants
{
    public static class JsonSerializerConstants
    {
        public static JsonSerializerOptions CamelOptions = new JsonSerializerOptions { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
    }
}
