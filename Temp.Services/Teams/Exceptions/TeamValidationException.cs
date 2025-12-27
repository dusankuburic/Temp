using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Teams.Exceptions;

public class TeamValidationException : ValidationEx
{
    public TeamValidationException(Exception innerException)
        : base("Team validation failed", new Dictionary<string, string[]>
        {
            { "Error", new[] { innerException.Message } }
        }) {
    }

    public TeamValidationException(string fieldName, string errorMessage)
        : base($"Team validation failed: {errorMessage}", fieldName, errorMessage) {
    }
}