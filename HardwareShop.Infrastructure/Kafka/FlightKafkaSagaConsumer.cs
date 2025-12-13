
using System.Text.Json;
using Confluent.Kafka;
using HardwareShop.Infrastructure.Data;
using HardwareShop.Infrastructure.Outbox;
using HardwareShop.Infrastructure.Saga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using UuidV7 = UUIDNext.Uuid;
namespace HardwareShop.Infrastructure.Kafka
{
    public class FlightBookedData : SagaData
    {
        public Guid FlightId { get; set; }
    }
    public class FlightKafkaSagaConsumer : BackgroundService
    {
        private readonly IServiceProvider provider;
        // private readonly IConsumer<string, string> consumer;
        private readonly ILogger<FlightKafkaSagaConsumer> logger;
        private readonly string[] topics = new[] { BookingSagaTopics.FlightBook };
        private int count = 0;
        public FlightKafkaSagaConsumer(IServiceProvider provider, IConfiguration config, ILogger<FlightKafkaSagaConsumer> logger)
        {
            this.provider = provider;
            this.logger = logger;
            logger.LogInformation("FlightKafkaSagaConsumer initialized.");
                    consumer = new ConsumerBuilder<string, string>(
                        new ConsumerConfig
                        {
                            BootstrapServers = config["Kafka:BootstrapServers"],
                            GroupId = "asfasf-v2",
                            EnableAutoCommit = false,
                            AutoOffsetReset = AutoOffsetReset.Earliest,


                        }).SetErrorHandler((_, e) =>
                        {

                            logger.LogError("Kafka error: {Reason}", e.Reason);
                        }).SetLogHandler((_, log) =>
            {
                logger.LogInformation(
                    "Kafka log: {Facility} - {Message}",
                    log.Facility,
                    log.Message
                );
            }).SetPartitionsAssignedHandler((c, partitions) =>
            {
                logger.LogInformation("Partitions assigned: {Partitions}",
                    string.Join(", ", partitions.Select(p => p.Partition.Value)));
            })
            Add revocation handler
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                logger.LogInformation("Partitions revoked: {Partitions}",
                    string.Join(", ", partitions.Select(p => p.Partition.Value)));
            }).Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("FlightKafkaSagaConsumer starting execution.");
            consumer.Subscribe(new[] { BookingSagaTopics.FlightBook, BookingSagaTopics.FlightCancel });
            return Task.Run(() => Listen(stoppingToken), stoppingToken);

        }
       
        private async Task Listen(CancellationToken ct)
        {

            while (!ct.IsCancellationRequested)
            {
                var result = consumer.Consume(ct);

                if (result == null)
                {
                    logger.LogInformation("No message received yet...");
                    continue;
                }
                var payload = JsonSerializer.Deserialize<FlightBookingData>(result.Message.Value);
                if (payload == null)
                {
                    logger.LogWarning("FlightKafkaSagaConsumer received null payload.");
                    consumer.Commit(result);
                    continue;
                }
                using var scope = provider.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<MainDatabaseContext>();
                if (count % 3 == 0)
                {
                    var guid = UuidV7.NewSequential();
                    logger.LogInformation($"FlightSagaConsumer received message with key: {result.Message.Key}, value: {result.Message.Value}, assigned UUIDv7: {guid}");

                    FlightBookedData data = new FlightBookedData
                    {
                        SagaId = payload.SagaId,
                        FlightId = guid
                    };
                    var outboxMsg = data.CreateOutboxMessage(BookingSagaTopics.FlightBooked);
                    ctx.OutboxMessages.Add(outboxMsg);
                    await ctx.SaveChangesAsync(ct);

                }
                else
                {
                    logger.LogInformation($"FlightSagaConsumer simulating failure for message with key: {result.Message.Key}, value: {result.Message.Value}");

                    // Simulate failure by sending to DLQ
                    SagaData data = new SagaData
                    {
                        SagaId = payload.SagaId,
                    };
                    var outboxMsg = data.CreateOutboxMessage(BookingSagaTopics.BookingDLQ);
                    ctx.OutboxMessages.Add(outboxMsg);
                    await ctx.SaveChangesAsync(ct);
                }
                count++;
                consumer.Commit(result);
                await Task.Delay(100, ct); // Simulate processing time
            }


    }
}