using StackExchange.Redis;
using Temp.Services.Caching;

namespace Temp.API.Bootstrap;

public static class ProgramConfigurePersistence
{
    public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration) {

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"], sqlOptions => {

                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);

                sqlOptions.CommandTimeout(30);

                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            })

            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddSingleton<IConnectionMultiplexer>(x => {
            var opt = ConfigurationOptions.Parse(configuration["ConnectionStrings:Redis"]);
            opt.AbortOnConnectFail = false;
            opt.ConnectRetry = 3;
            opt.ConnectTimeout = 5000;
            return ConnectionMultiplexer.Connect(opt);
        });

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}