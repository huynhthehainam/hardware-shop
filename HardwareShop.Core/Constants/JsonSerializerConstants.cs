using System.Text.Json;

namespace HardwareShop.Core.Constants
{
    public static class JsonSerializerConstants
    {
        public static JsonSerializerOptions CamelOptions { get; set; } = new() { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
    }
}
