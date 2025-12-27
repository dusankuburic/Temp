using Temp.Services.Exceptions;

namespace Temp.Services.Organizations.Exceptions;

public class OrganizationDependencyException : DependencyException
{
    public OrganizationDependencyException(Exception innerException)
        : base("Organization service dependency error occurred", innerException) {
    }

    public OrganizationDependencyException(string message, Exception innerException)
        : base(message, innerException) {
    }
}