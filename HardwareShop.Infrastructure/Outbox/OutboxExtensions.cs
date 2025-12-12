

using System.Text.Json;

namespace HardwareShop.Infrastructure.Outbox
{
    public static class OutboxExtensions
    {
        public static OutboxMessage CreateOutboxMessage<T>(this T message)
        {
            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = typeof(T).FullName ?? typeof(T).Name,
                Payload = JsonSerializer.Serialize(message),
                OccurredAt = DateTime.UtcNow
            };
        }
    }
}