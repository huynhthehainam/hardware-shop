using HardwareShop.Infrastructure.Outbox;

namespace HardwareShop.Infrastructure.Kafka;

public interface IKafkaProducerService
{
    Task ProduceAsync(string topic, string key, string value, CancellationToken cancellationToken = default);
    Task ProduceOutboxMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : OutboxMessage;
}
