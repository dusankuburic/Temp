using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Temp.Services.Integrations.Azure.AzureStorage;

public class AzureStorageInitializer : BackgroundService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureStorageOptions _options;

    public AzureStorageInitializer(BlobServiceClient blobServiceClient, IOptions<AzureStorageOptions> options)
    {
        _blobServiceClient = blobServiceClient;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

            await ConfigureCorsAsync(stoppingToken);
            
            Console.WriteLine("Azure Storage Initialized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing Azure Storage: {ex.Message}");
            // We might want to rethrow or handle this depending on how critical storage is for startup
            // For now, logging error is sufficient as the app might still work for non-storage features
        }
    }

    private async Task ConfigureCorsAsync(CancellationToken ct)
    {
        try
        {
            var properties = await _blobServiceClient.GetPropertiesAsync(cancellationToken: ct);
            var cors = properties.Value.Cors;

            if (!cors.Any(r => r.AllowedOrigins == "*"))
            {
                cors.Add(new BlobCorsRule
                {
                    AllowedHeaders = "*",
                    AllowedMethods = "GET,HEAD,OPTIONS,PUT,POST,DELETE",
                    AllowedOrigins = "*",
                    ExposedHeaders = "*",
                    MaxAgeInSeconds = 3600
                });
                await _blobServiceClient.SetPropertiesAsync(properties.Value, cancellationToken: ct);
                Console.WriteLine("Azure Storage CORS configured.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to configure Azure Storage CORS: {ex.Message}");
        }
    }
}
