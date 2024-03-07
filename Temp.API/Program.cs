using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Temp.API.Bootstrap;
using Temp.API.Middleware;
using Temp.Services.Integrations.Azure.AzureStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureLogging();

builder.Services.AddMappingsCollection();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

builder.Services.AddScoped<IAzureStorageService>(opt => new AzureStorageService(builder.Configuration["ConnectionStrings:AzureConnection"]));
builder.Services.AddProgramServices();
builder.Services.ConfigureCORS();
builder.Services.AddAuthSetup(builder.Configuration);

builder.Services.ConfigureSwaggerDoc();

builder.Services.AddControllers()
    .ConfigureSerialization()
    .ConfigureFluentValidation();

builder.Services.AddHealthChecks();
builder.Services.AddDataProtection();

var app = builder.Build();
app.UseCors("CorsPolicy");
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var ctx = services.GetRequiredService<ApplicationDbContext>();
        ctx.Database.Migrate();

        Seed.SeedOrganizations(ctx);
        Seed.SeedGroups(ctx);
        Seed.SeedTeams(ctx);
        Seed.SeedEmploymentStatuses(ctx);
        Seed.SeedWorkplaces(ctx);
        Seed.SeedEmployees(ctx);
        Seed.SeedAdmins(ctx);
        Seed.SeedUsers(ctx);
        Seed.SeedModerators(ctx);

        Seed.SeedWorkplaces(ctx);

    } catch (Exception exMsg) {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(exMsg, "Migration error");
    }
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwaggerDoc();

app.UseHttpsRedirection();
app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions {
    AllowCachingResponses = false,
    ResultStatusCodes = {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});


app.MapControllers();

app.Run();
