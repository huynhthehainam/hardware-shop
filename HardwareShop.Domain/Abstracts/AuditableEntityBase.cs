using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Abstracts
{
    public abstract class AuditableEntityBase : EntityBase, ITrackingDate
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? LastModifiedBy { get; set; }
    }
}