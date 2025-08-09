
using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class CreateUnitCommand
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public double? StepNumber { get; set; }
        [Required]
        public double? CompareWithPrimaryUnit { get; set; }
        [Required]
        public int? UnitCategoryId { get; set; }
    }
}