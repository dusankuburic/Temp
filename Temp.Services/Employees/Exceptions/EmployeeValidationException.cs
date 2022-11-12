namespace Temp.Services.Employees.Exceptions;

public class EmployeeValidationException : Exception
{
    public EmployeeValidationException(Exception innerException)
        : base("Invalid input, contact support.", innerException) { }
}
