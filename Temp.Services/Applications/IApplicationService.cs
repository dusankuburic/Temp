using Temp.Services.Applications.Models.Command;
using Temp.Services.Applications.Models.Query;

namespace Temp.Services.Applications;

public interface IApplicationService
{
    Task<CreateApplication.Response> CreateApplication(CreateApplication.Request request);
    Task<UpdateApplicationStatus.Response> UpdateApplicationStatus(int id, UpdateApplicationStatus.Request request);
    Task<GetApplication.ApplicationViewModel> GetApplication(int id);
    Task<IEnumerable<GetUserApplications.ApplicationViewModel>> GetUserApplications(int id);
    Task<IEnumerable<GetTeamApplications.ApplicationViewModel>> GetTeamApplications(int teamId, int moderatorId);
}

