using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Applications.Exceptions;

public class InvalidApplicationException : ValidationEx
{
    public InvalidApplicationException(string parameterName, object parameterValue)
        : base($"Invalid application property: {parameterName}",
               parameterName,
               $"Invalid value: {parameterValue}") {
    }
}