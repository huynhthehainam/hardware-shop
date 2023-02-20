

using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class RoundNumberCommand
    {
        [Required]
        public double? Value { get; set; }
    }
}