using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Temp.API.Bootstrap;
using Temp.API.Middleware;
using Temp.Domain.Models.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureLogging();
builder.Services.AddMappingsCollection();
builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.AddProgramServices(builder.Configuration);
builder.Services.AddAuthSetup(builder.Configuration);
builder.Services.ConfigureSwaggerDoc();
builder.Services.AddControllers()
    .ConfigureSerialization()
    .ConfigureFluentValidation();
builder.Services.AddHealthChecks();
builder.Services.AddDataProtection();
builder.Services.ConfigureCORS();
builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = true;
});
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
        Seed.SeedWorkplaces(ctx);
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        if (userManager.Users.AnyAsync().Result == false) {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
            await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
            await roleManager.CreateAsync(new IdentityRole() { Name = "Moderator" });

            var user = new AppUser {
                DisplayName = "John",
                Email = "johndoe@test.com",
                UserName = "johndoe@test.com",
                LockoutEnabled = false,
            };

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.DisplayName)
            };

            var result =  await userManager.CreateAsync(user, "5B3jt4H8$$3t03E88");
            if (result.Succeeded) {
                await userManager.AddToRoleAsync(user, "Admin");
                await userManager.AddClaimsAsync(user, claims);
            }

            var employee = await ctx.Employees
                .Where(x => x.Id == 1)
                .FirstOrDefaultAsync();
            employee.AppUserId = user.Id;
            employee.IsAppUserActive = true;
            await ctx.SaveChangesAsync();
        }

    } catch (Exception exMsg) {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(exMsg, "Migration error");
    }
}
app.UseHttpLogging();
app.UseResponseCompression();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwaggerDoc();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");
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
