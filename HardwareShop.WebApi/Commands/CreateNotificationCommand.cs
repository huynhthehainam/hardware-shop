using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace HardwareShop.WebApi.Commands
{
    public class CreateNotificationCommand
    {

        public string? Message { get; set; }
        [Required]
        public string? Variant { get; set; }
        public string? Translation { get; set; }
        public JsonDocument? TranslationParams { get; set; }
    }
}