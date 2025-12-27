using Temp.Services.Exceptions;

namespace Temp.Services.Employees.Exceptions;

public class EmployeeNotFoundException : NotFoundException
{
    public EmployeeNotFoundException()
        : base("Employee", "unknown") {
    }

    public EmployeeNotFoundException(int employeeId)
        : base("Employee", employeeId) {
    }

    public EmployeeNotFoundException(string message)
        : base(message) {
    }
}