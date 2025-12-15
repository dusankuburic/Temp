using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Temp.API.Bootstrap;

public static class ProgramHealthChecksSetup
{
    public static IServiceCollection AddHealthChecksConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        // Add SQL Server health check if connection string is configured
        var sqlConnection = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(sqlConnection))
        {
            healthChecksBuilder.AddSqlServer(
                connectionString: sqlConnection,
                healthQuery: "SELECT 1;",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "sql", "ready" },
                timeout: TimeSpan.FromSeconds(5));
        }

        // Add Redis health check if connection string is configured
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            healthChecksBuilder.AddRedis(
                redisConnectionString: redisConnection,
                name: "redis",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "cache", "redis", "ready" },
                timeout: TimeSpan.FromSeconds(3));
        }

        // Add Azure Blob Storage health check if connection string is configured
        var azureConnection = configuration.GetConnectionString("AzureConnection");
        if (!string.IsNullOrEmpty(azureConnection))
        {
            services.AddSingleton(_ => new BlobServiceClient(azureConnection));
            healthChecksBuilder.AddAzureBlobStorage(
                name: "azure-blob-storage",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "storage", "azure", "ready" },
                timeout: TimeSpan.FromSeconds(5));
        }

        return services;
    }
}
