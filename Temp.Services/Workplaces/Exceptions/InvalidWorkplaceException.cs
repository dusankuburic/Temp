using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Workplaces.Exceptions;

public class InvalidWorkplaceException : ValidationEx
{
    public InvalidWorkplaceException(string parameterName, object parameterValue)
        : base($"Invalid workplace property: {parameterName}",
               parameterName,
               $"Invalid value: {parameterValue}") {
    }
}