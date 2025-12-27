using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Employees.Exceptions;

public class EmployeeValidationException : ValidationEx
{
    public EmployeeValidationException(Exception innerException)
        : base("Employee validation failed", new Dictionary<string, string[]>
        {
            { "Error", new[] { innerException.Message } }
        }) {
    }

    public EmployeeValidationException(string fieldName, string errorMessage)
        : base($"Employee validation failed: {errorMessage}", fieldName, errorMessage) {
    }
}