using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Groups.Exceptions;

public class GroupValidationException : ValidationEx
{
    public GroupValidationException(Exception innerException)
        : base("Group validation failed", new Dictionary<string, string[]>
        {
            { "Error", new[] { innerException.Message } }
        }) {
    }

    public GroupValidationException(string fieldName, string errorMessage)
        : base($"Group validation failed: {errorMessage}", fieldName, errorMessage) {
    }
}