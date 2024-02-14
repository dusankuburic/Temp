using Temp.Services.Applications.Models.Commands;
using Temp.Services.Applications.Models.Queries;

namespace Temp.Services.Applications;

public interface IApplicationService
{
    Task<CreateApplicationResponse> CreateApplication(CreateApplicationRequest request);
    Task<UpdateApplicationStatusResponse> UpdateApplicationStatus(UpdateApplicationStatusRequest request);
    Task<GetApplicationResponse> GetApplication(GetApplicationRequest request);
    Task<IEnumerable<GetUserApplicationsResponse>> GetUserApplications(GetUserApplicationsRequest request);
    Task<IEnumerable<GetTeamApplicationsResponse>> GetTeamApplications(GetTeamApplicationsRequest request);
}

