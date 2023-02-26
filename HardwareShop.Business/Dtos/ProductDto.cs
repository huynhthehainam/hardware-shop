namespace HardwareShop.Business.Dtos
{
    public class ProductWarehouseDto
    {
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public double Quantity { get; set; }
    }
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double? Mass { get; set; }
        public double? PricePerMass { get; set; }
        public double? PercentForFamiliarCustomer { get; set; }
        public double? PercentForCustomer { get; set; }
        public bool HasAutoCalculatePermission { get; set; } = false;
        public double? PriceForFamiliarCustomer { get; set; }
        public double PriceForCustomer { get; set; }
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public int[] ProductCategoryIds { get; set; } = new int[0];
        public List<CategoryDto> Categories { get; set; } =  new List<CategoryDto>();
        public string?[] ProductCategoryNames { get; set; } = new string?[0];
        public List<ProductWarehouseDto>? Warehouses { get; set; }
        public List<SimpleAssetDto>? Assets { get; set; }
    }
}
