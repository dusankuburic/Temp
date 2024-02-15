using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;

namespace Temp.Services.Organizations;

public interface IOrganizationService
{
    Task<CreateOrganizationResponse> CreateOrganization(CreateOrganizationRequest request);
    Task<string> GetInnerGroups(int id);
    Task<GetOrganizationResponse> GetOrganization(GetOrganizationRequest request);
    Task<IEnumerable<GetOrganizationResponse>> GetOrganizations();
    Task<UpdateOrganizationResponse> UpdateOrganization(UpdateOrganizationRequest request);
    Task<UpdateOrganizationStatusResponse> UpdateOrganizationStatus(UpdateOrganizationStatusRequest request);
}

