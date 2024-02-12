using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;

namespace Temp.Services.Organizations;

public interface IOrganizationService
{
    Task<CreateOrganization.Response> CreateOrganization(CreateOrganization.Request request);
    Task<string> GetInnerGroups(int id);
    Task<GetOrganization.OrganizationViewModel> GetOrganization(int id);
    Task<IEnumerable<GetOrganizations.OrganizationViewModel>> GetOrganizations();
    Task<UpdateOrganization.Response> UpdateOrganization(int id, UpdateOrganization.Request request);
    Task<bool> UpdateOrganizationStatus(int id);
}

