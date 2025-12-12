using System.Reflection;
using System.Text.Json;
using Confluent.Kafka;
using HardwareShop.Application.Services;
using HardwareShop.Infrastructure.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HardwareShop.Infrastructure.Kafka;

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<string, string> producer;
    private readonly ILogger<KafkaProducerService> logger;

    public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
    {
        this.logger = logger;
        var kafkaConfig = configuration.GetSection("Kafka");
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaConfig["BootstrapServers"],
            Acks = Acks.All,                // wait for all replicas (safer for write-back)
            EnableIdempotence = true,       // avoid duplicates
            MessageTimeoutMs = 5000
        };
        producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceOutboxMessageAsync<T>(T outbox, CancellationToken cancellationToken = default) where T : OutboxMessage
    {
        await ProduceAsync(outbox.Topic, outbox.Id.ToString(), outbox.Payload, cancellationToken);
    }
    public async Task ProduceAsync(string topic, string key, string value, CancellationToken cancellationToken = default)
    {
        var payload = value;

        try
        {
            var deliveryResult = await producer.ProduceAsync(
                topic,
                new Message<string, string> { Key = key, Value = payload }, cancellationToken);

            logger.LogInformation(
                $"✅ Delivered to {deliveryResult.TopicPartitionOffset}: {payload}");
        }
        catch (ProduceException<string, string> ex)
        {
            logger.LogError($"❌ Delivery failed: {ex.Error.Reason}");
            throw;
        }
    }

    public void Dispose()
    {
        producer?.Flush();
        producer?.Dispose();
    }


}