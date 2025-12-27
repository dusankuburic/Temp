using Temp.Domain.Models;
using Temp.Services.EmploymentStatuses.Models.Commands;
using Temp.Services.EmploymentStatuses.Models.Queries;

namespace Temp.Services.EmploymentStatuses.Mappings;

public class EmploymentStatusesMappingProfile : Profile
{
    public EmploymentStatusesMappingProfile() {
        CreateMap<CreateEmploymentStatusRequest, EmploymentStatus>();
        CreateMap<EmploymentStatus, CreateEmploymentStatusResponse>();
        CreateMap<EmploymentStatus, GetEmploymentStatusResponse>();
        CreateMap<EmploymentStatus, GetPagedEmploymentStatusesResponse>();
    }
}