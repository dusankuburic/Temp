using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Auth.Exceptions;

public class UserValidationException : ValidationEx
{
    public UserValidationException(Exception innerException)
        : base("User validation failed", new Dictionary<string, string[]>
        {
            { "Error", new[] { innerException.Message } }
        }) {
    }

    public UserValidationException(string fieldName, string errorMessage)
        : base($"User validation failed: {errorMessage}", fieldName, errorMessage) {
    }
}