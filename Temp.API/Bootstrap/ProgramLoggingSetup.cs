using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Temp.Services.Integrations.Loggings;

namespace Temp.API.Bootstrap;

public static class ProgramLoggingSetup
{
    public static IServiceCollection ConfigureLogging(this IServiceCollection services) {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        Log.Logger = logger;

        services.AddSingleton(Log.Logger);
        services.AddScoped<ILoggingBroker, LoggingBroker>();

        services.AddHttpLogging(logging => {
            logging.RequestHeaders.Add("Authorization");
            logging.LoggingFields = HttpLoggingFields.All;
            logging.MediaTypeOptions.AddText("application/javascript");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        });

        return services;
    }
}
