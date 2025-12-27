using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Auth.Exceptions;

public class InvalidUserException : ValidationEx
{
    public InvalidUserException(string message)
        : base(message, "User", message) {
    }

    public InvalidUserException(string parameterName, object parameterValue)
        : base($"Invalid user property: {parameterName}",
               parameterName,
               $"Invalid value: {parameterValue}") { }
}