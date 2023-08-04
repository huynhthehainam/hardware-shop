namespace HardwareShop.Application.Dtos
{
    public sealed class CreateUnitDto
    {
        public string Name { get; set; } = string.Empty;
        public double StepNumber { get; set; }
        public int UnitCategoryId { get; set; }
        public double CompareWithPrimaryUnit { get; set; }
    }
}