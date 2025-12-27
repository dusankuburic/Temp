using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Auth.Exceptions;

public class NullUserException : ValidationEx
{
    public NullUserException()
        : base("User cannot be null", "User", "User object is required") {
    }

    public NullUserException(string fieldName)
        : base($"{fieldName} cannot be null", fieldName, "Value is required") {
    }
}