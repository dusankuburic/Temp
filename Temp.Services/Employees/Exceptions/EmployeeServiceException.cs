using Temp.Services.Exceptions;

namespace Temp.Services.Employees.Exceptions;

public class EmployeeServiceException : ServiceException
{
    public EmployeeServiceException(Exception innerException)
        : base("Employee service error occurred", "EMPLOYEE_SERVICE_ERROR", innerException) {
    }

    public EmployeeServiceException(string message, Exception innerException)
        : base(message, "EMPLOYEE_SERVICE_ERROR", innerException) {
    }
}