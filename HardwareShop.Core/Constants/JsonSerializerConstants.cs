using System.Text.Json;

namespace HardwareShop.Core.Constants
{
    public static class JsonSerializerConstants
    {
        public static JsonSerializerOptions CamelOptions = new() { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
        public static JsonDocument DefaultDocumentValue = JsonDocument.Parse("{}");
    }
}
