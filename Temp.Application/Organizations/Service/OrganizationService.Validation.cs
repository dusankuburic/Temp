using System;
using Temp.Domain.Models;
using Temp.Domain.Models.Organizations.Exceptions;

namespace Temp.Application.Organizations.Service
{
    public partial class OrganizationService
    {

        public void ValidateOrganizationOnCreate(Organization organization)
        {
            ValidateOrganization(organization);
        }

        public void ValidateOrganization(Organization organization)
        {
            if(organization is null)
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

        public static bool IsInvalid(string input) => String.IsNullOrWhiteSpace(input);
    }
}
