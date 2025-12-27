using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Engagements.Exceptions;

public class EngagementValidationException : ValidationEx
{
    public EngagementValidationException(Exception innerException)
        : base("Engagement validation failed", new Dictionary<string, string[]>
        {
            { "Error", new[] { innerException.Message } }
        }) {
    }

    public EngagementValidationException(string fieldName, string errorMessage)
        : base($"Engagement validation failed: {errorMessage}", fieldName, errorMessage) {
    }
}