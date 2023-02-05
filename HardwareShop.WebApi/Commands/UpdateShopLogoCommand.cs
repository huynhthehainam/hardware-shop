using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class UpdateShopLogoCommand
    {
        [Required]
        public IFormFile? Logo { get; set; }
    }
}
