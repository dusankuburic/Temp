using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Applications.Exceptions;

public class ApplicationValidationException : ValidationEx
{
    public ApplicationValidationException(Exception innerException)
        : base("Application validation failed", new Dictionary<string, string[]>
        {
            { "Error", new[] { innerException.Message } }
        }) {
    }

    public ApplicationValidationException(string fieldName, string errorMessage)
        : base($"Application validation failed: {errorMessage}", fieldName, errorMessage) {
    }
}