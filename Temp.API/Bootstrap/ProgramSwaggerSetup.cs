using Microsoft.OpenApi.Models;

namespace Temp.API.Bootstrap;

public static class ProgramSwaggerSetup
{
    public static IServiceCollection ConfigureSwaggerDoc(this IServiceCollection services) {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt => {
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Jwt Auth Bearer Scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            opt.AddSecurityDefinition("Bearer", securitySchema);
            var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        securitySchema, new[] {"Bearer"}
                    }
                };
            opt.AddSecurityRequirement(securityRequirement);

        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDoc(this IApplicationBuilder app) {

        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}