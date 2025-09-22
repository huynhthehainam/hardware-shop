namespace HardwareShop.Application.Services;

public interface IKafkaProducerService
{
    Task ProduceAsync<T>(string topic, T message);
}
