namespace Temp.API.Bootstrap;

public static class ProgramConfigurePersistence
{
    public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration) {

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]));


        return services;
    }
}
