using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Engagements.Exceptions;

public class InvalidEngagementException : ValidationEx
{
    public InvalidEngagementException(string parameterName, object parameterValue)
        : base($"Invalid engagement property: {parameterName}",
               parameterName,
               $"Invalid value: {parameterValue}") {
    }
}