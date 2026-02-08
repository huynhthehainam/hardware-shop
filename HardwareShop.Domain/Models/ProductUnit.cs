
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models;

public class ProductUnit : AuditableEntityBase
{
    public ProductUnit()
    {
    }

    public ProductUnit(Action<object, string?> lazyLoader) : base(lazyLoader)
    {
    }
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;
    public bool IsBaseUnit { get; set; }
    public double ConversionFactor { get; set; }
    private ICollection<WarehouseProduct>? warehouseProducts;
    public ICollection<WarehouseProduct>? WarehouseProducts
    {
        get => lazyLoader?.Load(this, ref warehouseProducts);
        set => warehouseProducts = value;
    }

    private ICollection<OrderDetail>? orderDetails;
    public ICollection<OrderDetail>? OrderDetails
    {
        get => lazyLoader?.Load(this, ref orderDetails);
        set => orderDetails = value;
    }

    #region  UnitPriceProperties
    public double? Mass { get; set; }
    public double? PricePerMass { get; set; }
    public double? PercentForFamiliarCustomer { get; set; }
    public double? PercentForCustomer { get; set; }
    public double? PriceForFamiliarCustomer { get; set; }
    public double OriginalPrice { get; set; }
    public double PriceForCustomer { get; set; }
    public bool HasAutoCalculatePermission { get; set; }
    #endregion
}