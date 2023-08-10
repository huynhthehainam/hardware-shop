namespace HardwareShop.Domain.Interfaces
{
    public interface ITrackingDate
    {
        DateTime CreatedDate { get; set; }
        DateTime? LastModifiedDate { get; set; }
    }
}
