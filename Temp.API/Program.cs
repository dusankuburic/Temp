using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Temp.API.Bootstrap;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureLogging();

builder.Services.AddMappingsCollection();

builder.Services.AddProgramServices();

builder.Services.ConfigureCORS();

builder.Services.AddControllers()
    .ConfigureSerilizaiton()
    .ConfigureFluentValidation();

builder.Services.Configure<ApiBehaviorOptions>(opt => {
    opt.InvalidModelStateResponseFactory = ctx =>
        new BadRequestObjectResult(ctx.ModelState);
});

builder.Services.AddAuthSetup(builder.Configuration);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

builder.Services.AddHealthChecks();

var app = builder.Build();

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

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler(builder => {
    builder.Run(async context => {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null) {
            context.Response.AddApplicationError(error.Error.Message);
            await context.Response.WriteAsync(error.Error.Message);
        }
    });
});

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

app.UseCors("CorsPolicy");
app.MapControllers();

app.Run();
