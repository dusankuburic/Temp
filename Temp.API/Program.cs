using FluentValidation.AspNetCore;
using Temp.Services.Applications;
using Temp.Services.Applications.Mappings;
using Temp.Services.Applications.Models.Validators;
using Temp.Services.Employees;
using Temp.Services.Employees.Mappings;
using Temp.Services.EmploymentStatuses;
using Temp.Services.EmploymentStatuses.Mappings;
using Temp.Services.Engagements;
using Temp.Services.Engagements.Mappings;
using Temp.Services.Groups;
using Temp.Services.Groups.Mapping;
using Temp.Services.Organizations;
using Temp.Services.Organizations.Mapping;
using Temp.Services.Teams;
using Temp.Services.Teams.Mappings;
using Temp.Services.Workplaces;
using Temp.Services.Workplaces.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<ApplicationsMappingProfile>();
    cfg.AddProfile<EmployeesMappingProfile>();
    cfg.AddProfile<EmploymentStatusesMappingProfile>();
    cfg.AddProfile<EngagementsMappingProfile>();
    cfg.AddProfile<GroupsMappingProfile>();
    cfg.AddProfile<OrganizationsMappingProfile>();
    cfg.AddProfile<TeamsMappingProfile>();
    cfg.AddProfile<WorkpacesMappingProfile>();
});

builder.Services.AddScoped<IEmploymentStatusService, EmploymentStatusService>();
builder.Services.AddScoped<IEngagementService, EngagementService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IWorkplaceService, WorkplaceService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<ITeamService, TeamService>();

builder.Services.AddCors(opt => {
    opt.AddPolicy("CorsPolicy", policy => {
        policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("http://localhost:4200");
    });
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<CreateApplicationRequestValidator>());



builder.Services.Configure<ApiBehaviorOptions>(opt => {
    opt.InvalidModelStateResponseFactory = ctx =>
        new BadRequestObjectResult(ctx.ModelState);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(config => {
    config.AddPolicy("Admin", policyBuilder =>
        policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));

    config.AddPolicy("User", policyBuilder =>
        policyBuilder.RequireClaim(ClaimTypes.Role, "User"));

    config.AddPolicy("Moderator", policyBuilder =>
        policyBuilder.RequireClaim(ClaimTypes.Role, "Moderator"));
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

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

app.UseCors("CorsPolicy");

app.MapControllers();


app.Run();
