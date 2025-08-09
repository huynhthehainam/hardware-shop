
using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class SelectProductThumbnailCommand
    {
        [Required]
        public int? AssetId { get; set; }
    }
}