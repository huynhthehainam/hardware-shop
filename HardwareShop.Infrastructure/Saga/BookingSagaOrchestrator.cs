using System.Text.Json;
using HardwareShop.Infrastructure.Data;
using HardwareShop.Infrastructure.Kafka;

namespace HardwareShop.Infrastructure.Saga
{
    public static class BookingSagaState
    {
        public const string Started = "Started";
        public const string FlightBooked = "FlightBooked";
        public const string HotelBooked = "HotelBooked";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
    }
    public class BookingSagaOrchestrator
    {
        private readonly MainDatabaseContext db;
        private readonly IKafkaProducerService producer;

        public BookingSagaOrchestrator(MainDatabaseContext db, IKafkaProducerService producer)
        {
            this.db = db;
            this.producer = producer;
        }

        public async Task HandleEventAsync(string topic, string payload, CancellationToken ct)
        {
            var evt = JsonSerializer.Deserialize<Dictionary<string, string>>(payload);
            var sagaId = Guid.Parse(evt["SagaId"]);

            var saga = await db.SagaStates.FindAsync(sagaId);

            if (saga == null)
                return;

            switch (topic)
            {
                case "flight.booked":
                    await HandleFlightBooked(saga, ct);
                    break;

                case "flight.failed":
                    await HandleFlightFailed(saga, ct);
                    break;

                case "hotel.booked":
                    await HandleHotelBooked(saga, ct);
                    break;

                case "hotel.failed":
                    await HandleHotelFailed(saga, ct);
                    break;
            }
        }

        private async Task HandleFlightBooked(SagaState saga, CancellationToken ct)
        {
            if (saga.State != BookingSagaState.Started.ToString())
                return;
            saga.State = BookingSagaState.FlightBooked.ToString();
            saga.UpdatedAt = DateTime.UtcNow;

            // Next step: book hotel
            // await producer.ProduceAsync("hotel.book", new Message<string, string>
            // {
            //     Key = saga.Id.ToString(),
            //     Value = JsonSerializer.Serialize(new { SagaId = saga.Id })
            // }, ct);

            await db.SaveChangesAsync(ct);
        }

        private async Task HandleFlightFailed(SagaState saga, CancellationToken ct)
        {
            saga.State = BookingSagaState.Failed.ToString();
            saga.UpdatedAt = DateTime.UtcNow;

            // No compensating action needed because flight already failed
            await db.SaveChangesAsync(ct);
        }

        private async Task HandleHotelBooked(SagaState saga, CancellationToken ct)
        {
            saga.State = BookingSagaState.Completed.ToString();
            saga.UpdatedAt = DateTime.UtcNow;

            // Saga completed
            await db.SaveChangesAsync(ct);
        }

        private async Task HandleHotelFailed(SagaState saga, CancellationToken ct)
        {
            saga.State = BookingSagaState.Failed.ToString();
            saga.UpdatedAt = DateTime.UtcNow;

            // Compensating: cancel flight booking
            // await producer.ProduceAsync("flight.cancel", new Message<string, string?>
            // {
            //     Key = saga.Id.ToString(),
            //     Value = JsonSerializer.Serialize(new { SagaId = saga.Id })
            // }, ct);

            await db.SaveChangesAsync(ct);
        }
    }

}