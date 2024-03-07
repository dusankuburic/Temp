namespace Temp.API.Bootstrap;

public static class ProgramAuthSetup
{
    public static IServiceCollection AddAuthSetup(this IServiceCollection services, IConfiguration configuration) {

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(configuration.GetSection("AppSettings:Token").Value)),
                    ValidIssuer = configuration["AppSettings:Issuer"],
                    ValidateIssuer = true,
                    ValidateAudience = false
                };
            });

        services.AddAuthorization(config => {
            config.AddPolicy("Admin", policyBuilder =>
                policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));

            config.AddPolicy("User", policyBuilder =>
                policyBuilder.RequireClaim(ClaimTypes.Role, "User"));

            config.AddPolicy("Moderator", policyBuilder =>
                policyBuilder.RequireClaim(ClaimTypes.Role, "Moderator"));
        });

        return services;
    }
}
