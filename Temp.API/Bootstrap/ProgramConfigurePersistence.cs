using StackExchange.Redis;

namespace Temp.API.Bootstrap;

public static class ProgramConfigurePersistence
{
    public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration) {

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]));

        services.AddSingleton<IConnectionMultiplexer>(x => {
            var opt = ConfigurationOptions.Parse("localhost");
            return ConnectionMultiplexer.Connect(opt);
        });

        return services;
    }
}
