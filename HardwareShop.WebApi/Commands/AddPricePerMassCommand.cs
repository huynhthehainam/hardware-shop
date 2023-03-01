

using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class AddPricePerMassCommand
    {
        [Required]
        public List<int>? ProductIds { get; set; }
        [Required]
        public double? AmountOfCash { get; set; }
    }
}