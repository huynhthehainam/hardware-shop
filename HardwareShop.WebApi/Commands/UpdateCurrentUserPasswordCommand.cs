

using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class UpdatePasswordCommand
    {
        [Required]
        public string? OldPassword { get; set; }
        [Required]
        public string? NewPassword { get; set; }
    }
}