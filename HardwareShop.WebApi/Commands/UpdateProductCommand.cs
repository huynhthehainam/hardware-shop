

namespace HardwareShop.WebApi.Commands
{
    public class UpdateProductCommand
    {
        public string? Name { get; set; }
        public double? Mass { get; set; }
        public double? PricePerMass { get; set; }
        public double? PercentForFamiliarCustomer { get; set; }
        public double? PercentForCustomer { get; set; }
        public List<int>? CategoryIds { get; set; }
        public double? PriceForFamiliarCustomer { get; set; }
        public double? PriceForCustomer { get; set; }
        public int? UnitId { get; set; }
        public bool? HasAutoCalculatePermission { get; set; }
        public List<CreateProductWarehouseQuantityCommand>? Warehouses { get; set; }
    }
}