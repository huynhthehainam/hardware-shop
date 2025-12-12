

namespace HardwareShop.Infrastructure.Saga
{
    public class SagaState
    {
        public Guid Id { get; set; }
        public string Data { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}