

using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class AddPricePerMassCommand
    {
        [Required]
        public List<int>? CategoryIds { get; set; }
        [Required]
        public double? AmountOfCash { get; set; }
    }
}