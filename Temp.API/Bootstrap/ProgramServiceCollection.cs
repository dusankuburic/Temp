using Azure.Storage.Blobs;
using Temp.Database.UnitOfWork;
using Temp.Services.Abstractions;
using Temp.Services.Applications;
using Temp.Services.Auth;
using Temp.Services.Employees;
using Temp.Services.EmploymentStatuses;
using Temp.Services.Engagements;
using Temp.Services.Groups;
using Temp.Services.Integrations.Azure.AzureStorage;
using Temp.Services.Organizations;
using Temp.Services.Providers;
using Temp.Services.Teams;
using Temp.Services.Workplaces;

namespace Temp.API.Bootstrap;

public static class ProgramServiceCollection
{
    public static IServiceCollection AddProgramServices(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmploymentStatusService, EmploymentStatusService>();
        services.AddScoped<IEngagementService, EngagementService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IWorkplaceService, WorkplaceService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IIdentityProvider, IdentityProvider>();

        services.Configure<AzureStorageOptions>(
            configuration.GetSection(AzureStorageOptions.SectionName));

        services.AddSingleton(_ =>
            new BlobServiceClient(configuration["ConnectionStrings:AzureConnection"]));

        services.AddScoped<AzureStorageService>();
        services.AddScoped<IAzureStorageService>(sp => sp.GetRequiredService<AzureStorageService>());
        services.AddScoped<IStorageService>(sp => sp.GetRequiredService<AzureStorageService>());

        services.AddHostedService<AzureStorageInitializer>();

        return services;
    }
}