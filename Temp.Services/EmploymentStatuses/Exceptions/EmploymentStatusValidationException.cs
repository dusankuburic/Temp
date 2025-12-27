using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.EmploymentStatuses.Exceptions;

public class EmploymentStatusValidationException : ValidationEx
{
    public EmploymentStatusValidationException(Exception innerException)
        : base("EmploymentStatus validation failed", new Dictionary<string, string[]>
        {
            { "Error", new[] { innerException.Message } }
        }) {
    }

    public EmploymentStatusValidationException(string fieldName, string errorMessage)
        : base($"EmploymentStatus validation failed: {errorMessage}", fieldName, errorMessage) {
    }
}