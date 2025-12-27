using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.EmploymentStatuses.Exceptions;

public class InvalidEmploymentStatusException : ValidationEx
{
    public InvalidEmploymentStatusException(string parameterName, object parameterValue)
        : base($"Invalid employment status property: {parameterName}",
               parameterName,
               $"Invalid value: {parameterValue}") {
    }
}