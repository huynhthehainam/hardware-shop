using System.Text.Json;

namespace HardwareShop.Core.Constants
{
    public static class JsonSerializerConstants
    {
        public static JsonSerializerOptions CamelOptions = new()
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };
        public static JsonDocument DefaultDocumentValue = JsonDocument.Parse("{}");
    }
}
