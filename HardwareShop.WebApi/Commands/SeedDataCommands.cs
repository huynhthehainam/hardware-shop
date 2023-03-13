
using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class SeedUnitCommand
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        [Required]
        public double? Step { get; set; }
    }
}