using Temp.Services.Applications;
using Temp.Services.Auth;
using Temp.Services.Employees;
using Temp.Services.EmploymentStatuses;
using Temp.Services.Engagements;
using Temp.Services.Groups;
using Temp.Services.Integrations.Azure.AzureStorage;
using Temp.Services.Organizations;
using Temp.Services.Teams;
using Temp.Services.Workplaces;

namespace Temp.API.Bootstrap;

public static class ProgramServiceCollection
{
    public static IServiceCollection AddProgramServices(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<IEmploymentStatusService, EmploymentStatusService>();
        services.AddScoped<IEngagementService, EngagementService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IWorkplaceService, WorkplaceService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IAzureStorageService>(opt =>
            new AzureStorageService(configuration["ConnectionStrings:AzureConnection"]));

        return services;
    }
}
