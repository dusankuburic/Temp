﻿namespace Temp.API.Bootstrap;

public static class ProgramCorsSetup
{
    public static IServiceCollection ConfigureCORS(this IServiceCollection services) {
        services.AddCors(opt => {
            opt.AddPolicy("CorsPolicy", policy => {
                policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
                //.WithOrigins("http://localhost:4242");
            });
        });

        return services;
    }
}
