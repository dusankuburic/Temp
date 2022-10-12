using System;

namespace Temp.Domain.Models.Organizations.Exceptions;

public class OrganizationValidationException : Exception
{
    public OrganizationValidationException(Exception innerException)
        : base("Invalid input, contact support", innerException) {

    }
}
