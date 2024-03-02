using Temp.Services._Helpers;
using Temp.Services.EmploymentStatuses.Models.Commands;
using Temp.Services.EmploymentStatuses.Models.Queries;

namespace Temp.Services.EmploymentStatuses;

public interface IEmploymentStatusService
{
    Task<CreateEmploymentStatusResponse> CreateEmploymentStatus(CreateEmploymentStatusRequest request);
    Task<GetEmploymentStatusResponse> GetEmploymentStatus(GetEmploymentStatusRequest request);
    Task<UpdateEmploymentStatusResponse> UpdateEmplymentStatus(UpdateEmploymentStatusRequest request);
    Task<List<GetEmploymentStatusResponse>> GetEmploymentStatuses();
    Task<PagedList<GetPagedEmploymentStatusesResponse>> GetPagedEmploymentStatuses(GetPagedEmploymentStatusesRequest request);
    Task<UpdateEmploymentStatusStatusResponse> UpdateEmploymentStatusStatus(UpdateEmploymentStatusStatusRequest request);
    Task<bool> EmploymentStatusExists(string name);
}
