using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Applications.Exceptions;

public class NullApplicationException : ValidationEx
{
    public NullApplicationException()
        : base("Application cannot be null", "Application", "Application object is required") {
    }

    public NullApplicationException(string fieldName)
        : base($"{fieldName} cannot be null", fieldName, "Value is required") {
    }
}