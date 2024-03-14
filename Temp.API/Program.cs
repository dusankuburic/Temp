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

var app = builder.Build();

app.UseHttpLogging();
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
        //Seed.SeedAdmins(ctx);
        //Seed.SeedUsers(ctx);
        //Seed.SeedModerators(ctx);
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
        await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
        await roleManager.CreateAsync(new IdentityRole() { Name = "Moderator" });

        var user = new AppUser {
            DisplayName = "John",
            Email = "johndoe@test.com",
            UserName = "johndoe@test.com"
        };

        var claims = new List<Claim>(){
           new Claim(ClaimTypes.Role, "Admin"),
           new Claim(ClaimTypes.Name, user.DisplayName)
        };

        var result =  await userManager.CreateAsync(user, "DebelaDrolja100%");
        if (result.Succeeded) {
            await userManager.AddToRoleAsync(user, "Admin");
            await userManager.AddClaimsAsync(user, claims);
        }


        var res = await ctx.Employees.Where(x => x.Id == 1).FirstOrDefaultAsync();
        res.AppUserId = user.Id;
        await ctx.SaveChangesAsync();


    } catch (Exception exMsg) {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(exMsg, "Migration error");
    }
}
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
