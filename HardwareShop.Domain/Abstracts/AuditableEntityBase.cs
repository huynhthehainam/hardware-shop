using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Abstracts
{
    public abstract class AuditableEntityBase : EntityBase, ITrackingDate
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}