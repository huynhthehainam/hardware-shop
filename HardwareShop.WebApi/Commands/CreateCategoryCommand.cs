


using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class CreateCategoryCommand
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}