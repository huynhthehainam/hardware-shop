using System.Text.Json;
using HardwareShop.Infrastructure.Data;
using HardwareShop.Infrastructure.Kafka;
using HardwareShop.Infrastructure.Outbox;

namespace HardwareShop.Infrastructure.Saga
{
    public class SagaData
    {
        public required Guid SagaId { get; set; }
    }
    public static class BookingSagaState
    {
        public const string Started = "Started";
        public const string FlightBooked = "FlightBooked";
        public const string HotelBooked = "HotelBooked";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string CompensateFlight = "CompensateFlight";
    }
    public static class BookingSagaTopics
    {
        public const string FlightBook = "flight.book";
        public const string FlightBooked = "flight.booked";
        public const string FlightFailed = "flight.failed";
        public const string HotelBook = "hotel.book";
        public const string HotelBooked = "hotel.booked";
        public const string HotelFailed = "hotel.failed";
        public const string FlightCancel = "flight.cancel";
        public const string FlightCancelled = "flight.cancelled";
        public const string BookingDLQ = "booking.dlq";
    }
    public class FlightBookingData : SagaData
    {
        public DateTime BookingDate { get; set; }
    }
    public class HotelBookingData : SagaData
    {
        public required Guid FlightId { get; set; }
        public DateTime BookingDate { get; set; }
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

        public async Task StartSagaAsync(Guid sagaId, CancellationToken ct)
        {
            var data = new FlightBookingData
            {
                BookingDate = DateTime.UtcNow,
                SagaId = sagaId
            };
            var saga = new SagaState
            {
                Id = sagaId,
                State = BookingSagaState.Started,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(data)
            };

            db.SagaStates.Add(saga);
            var outbox = data.CreateOutboxMessage(BookingSagaTopics.FlightBook);
            db.OutboxMessages.Add(outbox);
            await db.SaveChangesAsync(ct);
        }
        public async Task HandleEventAsync(string topic, string payload, CancellationToken ct)
        {
            var evt = JsonSerializer.Deserialize<SagaData>(payload);
            if (evt == null)
                return;
            var sagaId = evt.SagaId;

            var saga = await db.SagaStates.FindAsync(sagaId);

            if (saga == null)
                return;

            switch (topic)
            {
                case BookingSagaTopics.FlightBooked:
                    await HandleFlightBooked(saga, payload, ct);
                    break;

                case BookingSagaTopics.FlightFailed:
                    await HandleFlightFailed(saga, ct);
                    break;

                case BookingSagaTopics.HotelBooked:
                    await HandleHotelBooked(saga, ct);
                    break;

                case BookingSagaTopics.HotelFailed:
                    await HandleHotelFailed(saga, ct);
                    break;
            }
        }

        private async Task HandleFlightBooked(SagaState saga, string evtPayload, CancellationToken ct)
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
            var evt = JsonSerializer.Deserialize<FlightBookedData>(evtPayload);
            if (evt == null)
                return;
            var data = new HotelBookingData
            {
                FlightId = evt.FlightId,
                BookingDate = DateTime.UtcNow,
                SagaId = saga.Id
            };
            saga.Data = JsonSerializer.Serialize(data);
            var outbox = data.CreateOutboxMessage(BookingSagaTopics.HotelBook);
            db.OutboxMessages.Add(outbox);

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
            saga.State = BookingSagaState.CompensateFlight.ToString();
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