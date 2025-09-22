

using System.Linq.Expressions;
using System.Text.Json;
using HardwareShop.Application.Services;
using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace HardwareShop.Infrastructure.Services
{
    public class TestService : ITestService
    {
        private readonly IDistributedCache distributedCache;
        private readonly IKafkaProducerService kafkaProducerService;
        private readonly DbContext db;
        public TestService(DbContext db, IDistributedCache distributedCache, IKafkaProducerService kafkaProducerService)
        {
            this.db = db;
            this.distributedCache = distributedCache;
            this.kafkaProducerService = kafkaProducerService;
        }
        public async Task<int> TestWriteBackAsync()
        {
            var ticket = new Ticket
            {
                // Let EF Core assign Id when persisted
                CreatedDate = DateTime.UtcNow,
            };

            // Generate a temporary cache key (simulate Id before DB persist)
            var cacheKey = $"ticket:{Guid.NewGuid()}";

            // Step 2: Write ticket to cache first
            var ticketJson = JsonSerializer.Serialize(ticket);
            await distributedCache.SetStringAsync(
                cacheKey,
                ticketJson,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // keep it short-lived
                });

            // Step 3: Publish event to Kafka for async DB persistence
            await kafkaProducerService.ProduceAsync("ticket-writeback", new
            {
                CacheKey = cacheKey,
                Ticket = ticket
            });

            // Step 4: Return success quickly (simulate an Id)
            return ticket.Id;
        }
        public async Task<List<string?>> TestEncryptedAsync(CancellationToken cancellationToken)
        {
            var users = await db.Set<User>().Select(e => e.SecretValue).ToListAsync(cancellationToken);
            return users;
        }

        public async Task<int> TestEntityAsync()
        {
            Expression<Func<User, bool>> query = e => true;
            var countTask = db.Set<User>().CountAsync(query);
            var dataTask = db.Set<User>().Where(query).ToListAsync();

            var count = await countTask;
            var data = await dataTask;

            return count;
        }


    }
}