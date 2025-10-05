
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using HardwareShop.Infrastructure.Data;

namespace HardwareShop.KafkaConsumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly IConsumer<Null, string> consumer;
    private readonly IDistributedCache cache;
    private readonly MainDatabaseContext db;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IDistributedCache cache, MainDatabaseContext db)
    {
        this.logger = logger;
        this.cache = cache;
        this.db = db;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = configuration["Kafka:GroupId"] ?? "dotnet-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        consumer = new ConsumerBuilder<Null, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Subscribe("ticket-writeback");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                if (result != null)
                {
                    logger.LogInformation(
                        "Consumed message: {Key} - {Value} at: {Partition}:{Offset}",
                        result.Message.Key,
                        result.Message.Value,
                        result.Partition,
                        result.Offset
                    );

                    // Parse message to get cache key
                    var payload = JsonDocument.Parse(result.Message.Value);
                    if (!payload.RootElement.TryGetProperty("CacheKey", out var cacheKeyElement))
                    {
                        logger.LogWarning("Message missing CacheKey");
                        continue;
                    }
                    var cacheKey = cacheKeyElement.GetString();
                    if (string.IsNullOrEmpty(cacheKey))
                    {
                        logger.LogWarning("CacheKey is null or empty");
                        continue;
                    }

                    // Get ticket from cache
                    var ticketJson = await cache.GetStringAsync(cacheKey, stoppingToken);
                    if (ticketJson == null)
                    {
                        logger.LogWarning("Ticket not found in cache for key: {CacheKey}", cacheKey);
                        continue;
                    }

                    var ticket = JsonSerializer.Deserialize<Ticket>(ticketJson);
                    if (ticket == null)
                    {
                        logger.LogWarning("Failed to deserialize ticket from cache for key: {CacheKey}", cacheKey);
                        continue;
                    }

                    // Write ticket to database
                    db.Set<Ticket>().Add(ticket);
                    await db.SaveChangesAsync(stoppingToken);
                    logger.LogInformation("Ticket written to database with Id: {Id}", ticket.Id);

                    // Remove from cache
                    await cache.RemoveAsync(cacheKey, stoppingToken);
                    logger.LogInformation("Ticket removed from cache: {CacheKey}", cacheKey);
                }
            }
            catch (ConsumeException ex)
            {
                logger.LogError(ex, "Error while consuming Kafka message");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in write-back processing");
            }

            await Task.Delay(100, stoppingToken); // avoid tight loop
        }
    }

    public override void Dispose()
    {
        consumer.Close();
        consumer.Dispose();
        base.Dispose();
    }
}