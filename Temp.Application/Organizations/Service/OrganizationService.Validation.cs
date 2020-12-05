using System;
using System.Collections.Generic;
using System.Linq;
using Temp.Domain.Models;
using Temp.Domain.Models.Organizations.Exceptions;

namespace Temp.Application.Organizations.Service
{
    public partial class OrganizationService
    {

        public void ValidateOrganizationOnCreate(Organization organization)
        {
            ValidateOrganization(organization);
            ValidateOrganizationString(organization);
        }

        public void ValidateOrganizationOnUpdate(Organization organization)
        {
            ValidateOrganization(organization);
            ValidateOrganizationString(organization);
        }

        public void ValidateOrganization(Organization organization)
        {
            if(organization is null)
            {
                throw new NullOrganizationException();
            }
        }

        public void ValidateGetOrganizationViewModel(GetOrganization.OrganizationViewModel organization)
        {
            if (organization is null)
            {
                throw new NullOrganizationException();
            }
        }

        public void ValidateOrganizationString(Organization organization)
        {
            switch(organization)
            {
                case { } when IsInvalid(organization.Name):
                    throw new InvalidOrganizationException(
                        parameterName: nameof(organization.Name),
                        parameterValue: organization.Name);
            }
        }

        public void ValidateStorageOrganizations(IEnumerable<GetOrganizations.OrganizationViewModel> storageOrganizations)
        {
            if(storageOrganizations.Count() == 0)
            {
                throw new OrganizationEmptyStorageException();
            }
        }

        public static bool IsInvalid(string input) => String.IsNullOrWhiteSpace(input);
    }
}
