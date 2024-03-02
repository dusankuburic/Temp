using Temp.Services._Helpers;
using Temp.Services.Workplaces.Models.Commands;
using Temp.Services.Workplaces.Models.Queries;

namespace Temp.Services.Workplaces;

public interface IWorkplaceService
{
    Task<CreateWorkplaceResponse> CreateWorkplace(CreateWorkplaceRequest request);
    Task<PagedList<GetWorkplacesResponse>> GetPagedWorkplaces(GetWorkplacesRequest request);
    Task<List<GetWorkplaceResponse>> GetWorkplaces();
    Task<GetWorkplaceResponse> GetWorkplace(int id);
    Task<UpdateWorkplaceResponse> UpdateWorkplace(UpdateWorkplaceRequest request);
    Task<UpdateWorkplaceStatusResponse> UpdateWorkplaceStatus(UpdateWorkplaceStatusRequest request);
    Task<bool> WorkplaceExists(string name);
}
