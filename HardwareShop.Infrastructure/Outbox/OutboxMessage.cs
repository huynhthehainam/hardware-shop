
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Infrastructure.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public bool Dispatched { get; set; } = false;
        public DateTime? DispatchedAt { get; set; }
        public int Attempt { get; set; } = 0;
    }
}