using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Organizations.Exceptions;

public class NullOrganizationException : ValidationEx
{
    public NullOrganizationException()
        : base("Organization cannot be null", "Organization", "Organization object is required") {
    }

    public NullOrganizationException(string fieldName)
        : base($"{fieldName} cannot be null", fieldName, "Value is required") {
    }
}