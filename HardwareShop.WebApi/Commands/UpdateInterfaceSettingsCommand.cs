

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace HardwareShop.WebApi.Commands
{
    public class UpdateInterfaceSettingsCommand
    {
        [Required]
        public JsonDocument? Settings { get; set; }
    }
}