
using System.Text.Json;
using Confluent.Kafka;
using HardwareShop.Infrastructure.Data;
using HardwareShop.Infrastructure.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UuidV7 = UUIDNext.Uuid;
namespace HardwareShop.Infrastructure.Saga
{
    public class HotelBookedData : SagaData
    {
        public Guid HotelId { get; set; }
    }
    public class HotelKafkaSagaConsumer : BackgroundService
    {
        private readonly IServiceProvider provider;
        private readonly IConsumer<string, string> consumer;
        public HotelKafkaSagaConsumer(IServiceProvider provider, IConfiguration config)
        {
            this.provider = provider;
            consumer = new ConsumerBuilder<string, string>(
                new ConsumerConfig
                {
                    BootstrapServers = config["Kafka:BootstrapServers"],
                    GroupId = "hotel-saga-consumer",
                    EnableAutoCommit = false,
                }).Build();
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            consumer.Subscribe([BookingSagaTopics.HotelBook]);
            return Task.Run(() => Listen(stoppingToken), stoppingToken);
        }
        private async Task Listen(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var result = consumer.Consume(ct);
                var payload = JsonSerializer.Deserialize<FlightBookingData>(result.Message.Value);
                if (payload == null)
                {
                    consumer.Commit(result);
                    continue;
                }
                using var scope = provider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<MainDatabaseContext>();
                var guid = UuidV7.NewSequential();
                HotelBookedData data = new HotelBookedData
                {
                    SagaId = payload.SagaId,
                    HotelId = guid
                };
                var outboxMsg = data.CreateOutboxMessage(BookingSagaTopics.HotelBooked);
                ctx.OutboxMessages.Add(outboxMsg);
                await ctx.SaveChangesAsync(ct);

                consumer.Commit(result);
            }
        }
    }
}