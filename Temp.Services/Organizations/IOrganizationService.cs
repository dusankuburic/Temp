using Temp.Services._Helpers;
using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;

namespace Temp.Services.Organizations;

public interface IOrganizationService
{
    Task<CreateOrganizationResponse> CreateOrganization(CreateOrganizationRequest request);
    Task<PagedList<GetOrganizationResponse>> GetPagedOrganizations(GetOrganizationsRequest request);
    Task<GetPagedInnerGroupsResponse> GetPagedInnerGroups(GetOrganizationInnerGroupsRequest request);
    Task<List<InnerGroup>> GetInnerGroups(int id);
    Task<GetOrganizationResponse> GetOrganization(GetOrganizationRequest request);
    Task<IEnumerable<GetOrganizationResponse>> GetOrganizations();
    Task<UpdateOrganizationResponse> UpdateOrganization(UpdateOrganizationRequest request);
    Task<UpdateOrganizationStatusResponse> UpdateOrganizationStatus(UpdateOrganizationStatusRequest request);
    Task<bool> OrganizationExists(string name);
}