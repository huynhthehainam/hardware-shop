
using System.Reflection;
using HardwareShop.Application.Services;
using HardwareShop.Infrastructure.Data;
using HardwareShop.Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HardwareShop.Infrastructure.Outbox
{
    public class OutboxDispatcher : BackgroundService
    {
        private readonly IServiceProvider sp;
        private readonly ILogger<OutboxDispatcher> logger;
        private readonly TimeSpan interval = TimeSpan.FromSeconds(2);

        public OutboxDispatcher(IServiceProvider sp, ILogger<OutboxDispatcher> logger)
        {
            this.sp = sp;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("OutboxDispatcher started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = sp.CreateScope();
                    var ctx = scope.ServiceProvider.GetRequiredService<MainDatabaseContext>();
                    var bus = scope.ServiceProvider.GetRequiredService<IKafkaProducerService>();


                    // fetch a batch of undelivered messages
                    var batch = await ctx.OutboxMessages
                    .Where(o => !o.Dispatched)
                    .OrderBy(o => o.OccurredAt)
                    .Take(20)
                    .ToListAsync(stoppingToken);


                    foreach (var outbox in batch)
                    {
                        try
                        {
                            await bus.ProduceOutboxMessageAsync(outbox, stoppingToken);
                            outbox.Dispatched = true;
                            outbox.DispatchedAt = DateTime.UtcNow;
                            outbox.Attempt += 1;
                            ctx.OutboxMessages.Update(outbox);
                            await ctx.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            outbox.Attempt += 1;
                            ctx.OutboxMessages.Update(outbox);
                            await ctx.SaveChangesAsync(stoppingToken);
                            logger.LogError(ex, "Failed to dispatch outbox message {Id}", outbox.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while running outbox dispatcher loop");
                }


                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}