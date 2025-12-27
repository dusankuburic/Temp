using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Employees.Exceptions;

public class InvalidEmployeeException : ValidationEx
{
    public InvalidEmployeeException(string parameterName, object parameterValue)
        : base($"Invalid employee property: {parameterName}",
               parameterName,
               $"Invalid value: {parameterValue}") {
    }
}