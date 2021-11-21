using System;

namespace Temp.Domain.Models.Organizations.Exceptions
{
    public class OrganizationDependencyException : Exception
    {
        public OrganizationDependencyException(Exception innerException)
            : base("Service dependency error occurred, contact support", innerException) {

        }
    }
}