

using System.Text.Json;

namespace HardwareShop.Infrastructure.Outbox
{
    public static class OutboxExtensions
    {
        public static OutboxMessage CreateOutboxMessage<T>(this T message, string topic)
        {
            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Topic = topic,
                Payload = JsonSerializer.Serialize(message),
                OccurredAt = DateTime.UtcNow
            };
        }
    }
}