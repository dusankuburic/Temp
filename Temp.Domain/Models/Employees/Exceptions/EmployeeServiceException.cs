namespace Temp.Domain.Models.Employees.Exceptions;

public class EmployeeServiceException : Exception
{
    public EmployeeServiceException(Exception innerException)
        : base("Service error, contact support", innerException) {

    }
}
