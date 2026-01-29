
namespace HardwareShop.Domain.Abstracts
{
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    }
}