using Temp.Core._Helpers;
using Temp.Services.Workplaces.Models.Command;
using Temp.Services.Workplaces.Models.Query;

namespace Temp.Services.Workplaces;

public interface IWorkplaceService
{
    Task<CreateWorkplaceResponse> CreateWorkplace(CreateWorkplaceRequest request);
    Task<PagedList<GetWorkplacesResponse>> GetPagedWorkplaces(GetWorkplacesRequest request);
    Task<IEnumerable<GetWorkplaceResponse>> GetWorkplaces();
    Task<GetWorkplaceResponse> GetWorkplace(int id);
    Task<UpdateWorkplaceResponse> UpdateWorkplace(UpdateWorkplaceRequest request);
    Task<UpdateWorkplaceStatusResponse> UpdateWorkplaceStatus(UpdateWorkplaceStatusRequest request);
}
