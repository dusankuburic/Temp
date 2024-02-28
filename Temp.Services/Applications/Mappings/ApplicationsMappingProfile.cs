using Temp.Domain.Models.Applications;
using Temp.Services.Applications.Models.Commands;
using Temp.Services.Applications.Models.Queries;

namespace Temp.Services.Applications.Mappings;

public class ApplicationsMappingProfile : Profile
{
    public ApplicationsMappingProfile() {
        CreateMap<CreateApplicationRequest, Application>();

        CreateMap<Application, CreateApplicationResponse>();
        CreateMap<Application, GetApplicationResponse>();
        CreateMap<Application, GetUserApplicationsResponse>();
        CreateMap<Application, GetTeamApplicationsResponse>()
            .ForMember(a => a.Username, o => o.MapFrom(s => s.User.Username));

        CreateMap<UpdateApplicationStatusRequest, Application>()
            .AfterMap((x, y) => {
                y.Status = true;
                y.StatusUpdatedAt = DateTime.UtcNow;
            });
    }
}

