using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Temp.API.Bootstrap;

public static class ProgramHealthChecksSetup
{
    public static IServiceCollection AddHealthChecksConfiguration(
        this IServiceCollection services,
        IConfiguration configuration) {
        var healthChecksBuilder = services.AddHealthChecks();

        var sqlConnection = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(sqlConnection)) {
            healthChecksBuilder.AddSqlServer(
                connectionString: sqlConnection,
                healthQuery: "SELECT 1;",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["db", "sql", "ready"],
                timeout: TimeSpan.FromSeconds(5));
        }

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection)) {
            healthChecksBuilder.AddRedis(
                redisConnectionString: redisConnection,
                name: "redis",
                failureStatus: HealthStatus.Degraded,
                tags: ["cache", "redis", "ready"],
                timeout: TimeSpan.FromSeconds(3));
        }

        var azureConnection = configuration.GetConnectionString("AzureConnection");
        if (!string.IsNullOrEmpty(azureConnection)) {
            services.AddSingleton(_ => new BlobServiceClient(azureConnection));
            healthChecksBuilder.AddAzureBlobStorage(
                name: "azure-blob-storage",
                failureStatus: HealthStatus.Degraded,
                tags: ["storage", "azure", "ready"],
                timeout: TimeSpan.FromSeconds(5));
        }

        return services;
    }
}