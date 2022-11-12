namespace Temp.Services.Employees.Exceptions;

public class NullEmployeeException : Exception
{
    public NullEmployeeException()
        : base("The employee is null.") { }
}
