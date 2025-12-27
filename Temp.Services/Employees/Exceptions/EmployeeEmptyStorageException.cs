using Temp.Services.Exceptions;

namespace Temp.Services.Employees.Exceptions;

public class EmployeeEmptyStorageException : NotFoundException
{
    public EmployeeEmptyStorageException()
        : base("No employees found in storage") {
    }
}