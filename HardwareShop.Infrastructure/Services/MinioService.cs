using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;

namespace HardwareShop.Infrastructure.Services;

public interface IMinioService
{
    Task<byte[]> GetFileBytesByKeyAsync(string bucket, string key);
}

public class MinioService : IMinioService
{
    private readonly IMinioClient minioClient;

    public MinioService(IConfiguration configuration)
    {
        var endpoint = configuration["Minio:Endpoint"] ?? "localhost:9000";
        var accessKey = configuration["Minio:AccessKey"] ?? "admin";
        var secretKey = configuration["Minio:SecretKey"] ?? "admin123456";

        var useSsl = bool.TryParse(configuration["Minio:UseSsl"], out var parsedUseSsl) && parsedUseSsl;

        minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(useSsl)
            .Build();
    }

    public async Task<byte[]> GetFileBytesByKeyAsync(string bucket, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("File key is required.", nameof(key));
        }

        using var memoryStream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(key)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await minioClient.GetObjectAsync(getObjectArgs);
        return memoryStream.ToArray();
    }
}
