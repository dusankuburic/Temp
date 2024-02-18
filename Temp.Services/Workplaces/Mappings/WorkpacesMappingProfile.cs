using Temp.Domain.Models;
using Temp.Services.Workplaces.Models.Commands;
using Temp.Services.Workplaces.Models.Queries;

namespace Temp.Services.Workplaces.Mappings;

public class WorkpacesMappingProfile : Profile
{
    public WorkpacesMappingProfile() {
        CreateMap<CreateWorkplaceRequest, Workplace>();
        CreateMap<Workplace, CreateWorkplaceResponse>();
        CreateMap<Workplace, GetWorkplacesResponse>();
        CreateMap<Workplace, GetWorkplaceResponse>();
        CreateMap<UpdateWorkplaceRequest, Workplace>();
    }
}
