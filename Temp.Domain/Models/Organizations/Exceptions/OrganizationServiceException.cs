namespace Temp.Domain.Models.Organizations.Exceptions;

public class OrganizationServiceException : Exception
{
    public OrganizationServiceException(Exception innerException)
        : base("Service error, contact support", innerException) {

    }
}
