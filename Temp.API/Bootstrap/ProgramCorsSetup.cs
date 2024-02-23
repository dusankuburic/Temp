namespace Temp.API.Bootstrap;

public static class ProgramCorsSetup
{
    public static IServiceCollection ConfigureCORS(this IServiceCollection services) {
        services.AddCors(opt => {
            opt.AddPolicy("CorsPolicy", policy => {
                policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("http://localhost:4200");
            });
        });

        return services;
    }
}
