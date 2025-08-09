
using System.ComponentModel.DataAnnotations;

namespace HardwareShop.WebApi.Commands
{
    public class SeedFromFileCommand
    {
        [Required]
        public IFormFile? DbFile { get; set; }
        [Required]
        public int? ShopId { get; set; }
    }
    public class SeedUnitCommand
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        [Required]
        public double? Step { get; set; }
    }
    public class SeedProductCommand
    {
        [Required]
        public string? Uuid { get; set; }
        [Required]
        public string? Name { get; set; }
        public double? Mass { get; set; }
        [Required]
        public int? UnitId { get; set; }
        public double? PricePerMass { get; set; }
        public double? PercentForFamiliarCustomer { get; set; }
        public double? PercentForCustomer { get; set; }
        public double? OriginalPrice { get; set; }
        public double? PriceForFamiliarCustomer { get; set; }
        [Required]
        public double? PriceForCustomer { get; set; }
        [Required]
        public int? ShopId { get; set; }
        public bool HasAutoCalculatePermission { get; set; } = true;
    }
    public class SeedCustomerDebtHistoryCommand
    {
        [Required]
        public DateTime? CreatedDate { get; set; }
        public string TimeZone { get; set; } = "SE Asia Standard Time";
        [Required]
        public double? Amount { get; set; }
        [Required]
        public string? Reason { get; set; }

    }
    public class SeedCustomerCommand
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public int? ShopId { get; set; }

        public string? Address { get; set; }
        [Required]
        public string? Uuid { get; set; }
        [Required]
        public double? Debt { get; set; }
        public List<SeedCustomerDebtHistoryCommand> Histories { get; set; } = new List<SeedCustomerDebtHistoryCommand>();
    }
}