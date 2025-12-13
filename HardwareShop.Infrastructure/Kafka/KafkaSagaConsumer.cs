using Confluent.Kafka;
using HardwareShop.Infrastructure.Saga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UuidV7 = UUIDNext.Uuid;
namespace HardwareShop.Infrastructure.Kafka
{
    public class KafkaSagaConsumer : BackgroundService
    {
        private readonly IServiceProvider provider;
        private readonly IConfiguration config;
        private IConsumer<string, string> consumer;

        public KafkaSagaConsumer(IServiceProvider provider, IConfiguration config)
        {
            this.provider = provider;
            this.config = config;

            consumer = new ConsumerBuilder<string, string>(
                new ConsumerConfig
                {
                    BootstrapServers = config["Kafka:BootstrapServers"],
                    GroupId = "saga-orchestrator",
                    EnableAutoCommit = false,
                }).Build();
        }

        protected override  Task ExecuteAsync(CancellationToken stoppingToken)
        {
            consumer.Subscribe([
            BookingSagaTopics.FlightBooked,
            BookingSagaTopics.FlightFailed,
            BookingSagaTopics.HotelBooked,
            BookingSagaTopics.HotelFailed,
            BookingSagaTopics.FlightCancelled,
            BookingSagaTopics.BookingDLQ
        ]);

            return Task.Run(() => Listen(stoppingToken), stoppingToken);
        }

        private async Task Listen(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var result = consumer.Consume(ct);

                using var scope = provider.CreateScope();
                var orchestrator = scope.ServiceProvider.GetRequiredService<BookingSagaOrchestrator>();

                await orchestrator.HandleEventAsync(result.Topic, result.Message.Value, ct);
                consumer.Commit(result);
            }
        }
    }

}