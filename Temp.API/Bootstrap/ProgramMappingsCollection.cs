using Temp.Services.Applications.Mappings;
using Temp.Services.Employees.Mappings;
using Temp.Services.EmploymentStatuses.Mappings;
using Temp.Services.Engagements.Mappings;
using Temp.Services.Groups.Mapping;
using Temp.Services.Organizations.Mapping;
using Temp.Services.Teams.Mappings;
using Temp.Services.Workplaces.Mappings;

namespace Temp.API.Bootstrap;

public static class ProgramMappingsCollection
{
    public static IServiceCollection AddMappingsCollection(this IServiceCollection services) {

        services.AddAutoMapper(cfg => {
            cfg.AddProfile<ApplicationsMappingProfile>();
            cfg.AddProfile<EmployeesMappingProfile>();
            cfg.AddProfile<EmploymentStatusesMappingProfile>();
            cfg.AddProfile<EngagementsMappingProfile>();
            cfg.AddProfile<GroupsMappingProfile>();
            cfg.AddProfile<OrganizationsMappingProfile>();
            cfg.AddProfile<TeamsMappingProfile>();
            cfg.AddProfile<WorkpacesMappingProfile>();
        });


        return services;
    }
}
