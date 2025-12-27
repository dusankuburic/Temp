using Temp.Services.Exceptions;

namespace Temp.Services.Employees.Exceptions;

public class EmployeeDependencyException : DependencyException
{
    public EmployeeDependencyException(Exception innerException)
        : base("Employee service dependency error occurred", innerException) {
    }

    public EmployeeDependencyException(string message, Exception innerException)
        : base(message, innerException) {
    }
}