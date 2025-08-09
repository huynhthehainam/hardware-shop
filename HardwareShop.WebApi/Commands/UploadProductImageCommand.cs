
using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class UploadProductImageCommand
    {
        [Required]
        public IFormFile? Image { get; set; }
        [Required]
        public string? AssetType { get; set; }
    }
}