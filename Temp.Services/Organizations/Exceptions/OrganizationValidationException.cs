using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Organizations.Exceptions;

public class OrganizationValidationException : ValidationEx
{
    public OrganizationValidationException(Exception innerException)
        : base("Organization validation failed", new Dictionary<string, string[]>
        {
            { "Error", new[] { innerException.Message } }
        }) {
    }

    public OrganizationValidationException(string fieldName, string errorMessage)
        : base($"Organization validation failed: {errorMessage}", fieldName, errorMessage) {
    }
}