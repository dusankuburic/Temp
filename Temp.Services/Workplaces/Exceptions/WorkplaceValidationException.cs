using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Workplaces.Exceptions;

public class WorkplaceValidationException : ValidationEx
{
    public WorkplaceValidationException(Exception innerException)
        : base("Workplace validation failed", new Dictionary<string, string[]>
        {
            { "Error", new[] { innerException.Message } }
        }) {
    }

    public WorkplaceValidationException(string fieldName, string errorMessage)
        : base($"Workplace validation failed: {errorMessage}", fieldName, errorMessage) {
    }
}