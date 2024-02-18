using Temp.Domain.Models;
using Temp.Services.Engagements.Models.Commands;
using Temp.Services.Engagements.Models.Queries;

namespace Temp.Services.Engagements.Mappings;

public class EngagementsMappingProfile : Profile
{
    public EngagementsMappingProfile() {
        CreateMap<CreateEngagementRequest, Engagement>();
        CreateMap<Engagement, CreateEngagementResponse>();
        CreateMap<Engagement, GetUserEmployeeEngagementsResponse>()
            .ForMember(a => a.WorkplaceName, o => o.MapFrom(s => s.Workplace.Name))
            .ForMember(a => a.EmploymentStatusName, o => o.MapFrom(s => s.EmploymentStatus.Name));
        CreateMap<Engagement, GetEngagementsForEmployeeResponse>()
            .ForMember(a => a.WorkplaceName, o => o.MapFrom(s => s.Workplace.Name))
            .ForMember(a => a.EmploymentStatusName, o => o.MapFrom(s => s.EmploymentStatus.Name));
    }
}
