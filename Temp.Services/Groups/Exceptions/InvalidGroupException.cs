using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Groups.Exceptions;

public class InvalidGroupException : ValidationEx
{
    public InvalidGroupException(string parameterName, object parameterValue)
        : base($"Invalid group property: {parameterName}",
               parameterName,
               $"Invalid value: {parameterValue}") {
    }
}