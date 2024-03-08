using Temp.Domain.Models;
using Temp.Services.Organizations.Exceptions;
using Temp.Services.Organizations.Models.Queries;

namespace Temp.Services.Organizations;

public partial class OrganizationService
{
    private void ValidateOrganizationOnCreate(Organization organization) {
        ValidateOrganization(organization);
        ValidateOrganizationString(organization);
    }

    private void ValidateOrganizationOnUpdate(Organization organization) {
        ValidateOrganization(organization);
        ValidateOrganizationString(organization);
    }

    private void ValidateOrganization(Organization organization) {
        if (organization is null) {
            throw new NullOrganizationException();
        }
    }

    private void ValidateGetOrganizationViewModel(GetOrganizationResponse organization) {
        if (organization is null) {
            throw new NullOrganizationException();
        }
    }

    private void ValidateOrganizationString(Organization organization) {
        switch (organization) {
            case { } when IsInvalid(organization.Name):
                throw new InvalidOrganizationException(
                    parameterName: nameof(organization.Name),
                    parameterValue: organization.Name);
        }
    }

    private void ValidateStorageOrganizations(IEnumerable<GetOrganizationResponse> storageOrganizations) {
        if (storageOrganizations.Count() == 0) {
            throw new OrganizationEmptyStorageException();
        }
    }

    private void ValidateStorageOrganizationInnerGroups(IEnumerable<GetInnerGroups.InnerGroupViewModel> innerGroupViewModels) {
        if (innerGroupViewModels.Count() == 0) {
            throw new OrganizationGetInnerGroupsStorageException();
        }
    }

    private static bool IsInvalid(string input) {
        return string.IsNullOrWhiteSpace(input);
    }
}

