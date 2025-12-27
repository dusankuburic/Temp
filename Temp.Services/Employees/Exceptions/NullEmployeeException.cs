using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Employees.Exceptions;

public class NullEmployeeException : ValidationEx
{
    public NullEmployeeException()
        : base("Employee cannot be null", "Employee", "Employee object is required") {
    }

    public NullEmployeeException(string fieldName)
        : base($"{fieldName} cannot be null", fieldName, "Value is required") {
    }
}