
namespace HardwareShop.Domain.Models;

public class ProductUnit
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;
    public bool IsBaseUnit { get; set; }
    public double ConversionFactor { get; set; }
}