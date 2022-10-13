namespace Temp.Services.Organizations.Exceptions;

public class OrganizationValidationException : Exception
{
    public OrganizationValidationException(Exception innerException)
        : base("Invalid input, contact support", innerException) {

    }
}

