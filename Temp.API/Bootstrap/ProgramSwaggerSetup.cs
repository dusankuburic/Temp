namespace Temp.API.Bootstrap;

public static class ProgramSwaggerSetup
{
    public static IServiceCollection ConfigureSwaggerDoc(this IServiceCollection services) {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IApplicationBuilder UseSwaggerDoc(this IApplicationBuilder app) {

        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
