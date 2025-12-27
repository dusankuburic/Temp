using Temp.Services.Exceptions;

namespace Temp.Services.Organizations.Exceptions;

public class OrganizationServiceException : ServiceException
{
    public OrganizationServiceException(Exception innerException)
        : base("Organization service error occurred", "ORGANIZATION_SERVICE_ERROR", innerException) {
    }

    public OrganizationServiceException(string message, Exception innerException)
        : base(message, "ORGANIZATION_SERVICE_ERROR", innerException) {
    }
}