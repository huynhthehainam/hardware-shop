

using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class CreateProductWarehouseQuantityCommand
    {
        public int WarehouseId { get; set; }
        public double Quantity { get; set; }
    }
    public class CreateProductCommand
    {
        [Required]
        public string? Name { get; set; }
        public double? Mass { get; set; }
        public double? PricePerMass { get; set; }
        public double? PercentForFamiliarCustomer { get; set; }
        public double? PercentForCustomer { get; set; }
        [Required]
        public List<int>? CategoryIds { get; set; }
        public double? PriceForFamiliarCustomer { get; set; }
        [Required]
        public double? PriceForCustomer { get; set; }
        [Required]
        public int? UnitId
        {
            get; set;
        }
        public bool HasAutoCalculatePermission { get; set; } = false;
        public List<CreateProductWarehouseQuantityCommand>? Warehouses { get; set; }

    }
}