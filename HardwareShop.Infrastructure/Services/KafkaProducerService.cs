using System.Text.Json;
using Confluent.Kafka;
using HardwareShop.Application.Services;
using Microsoft.Extensions.Configuration;

namespace HardwareShop.Infrastructure.Services;

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<Null, string> producer;
    private readonly string? defaultTopic;

    public KafkaProducerService(IConfiguration configuration)
    {
        var kafkaConfig = configuration.GetSection("Kafka");
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaConfig["BootstrapServers"],
            Acks = Acks.All,                // wait for all replicas (safer for write-back)
            EnableIdempotence = true,       // avoid duplicates
            MessageTimeoutMs = 5000
        };
        producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync<T>(string topic, T message)
    {
        var payload = JsonSerializer.Serialize(message);

        try
        {
            var deliveryResult = await producer.ProduceAsync(
                topic,
                new Message<Null, string> { Value = payload });

            Console.WriteLine(
                $"✅ Delivered to {deliveryResult.TopicPartitionOffset}: {payload}");
        }
        catch (ProduceException<Null, string> ex)
        {
            Console.WriteLine($"❌ Delivery failed: {ex.Error.Reason}");
            throw;
        }
    }

    public void Dispose()
    {
        producer?.Flush();
        producer?.Dispose();
    }
}