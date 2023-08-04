using System.Text.Json;

namespace HardwareShop.Application.Dtos
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Message { get; set; }
        public JsonDocument? Options { get; set; }
        public string? Translation { get; set; }
        public JsonDocument? TranslationParams { get; set; }
    }
}